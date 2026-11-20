using System;
using System.Collections.Generic;
using System.Linq;
using GPFA.Data;
using GPFA.Models;

namespace GPFA.Business
{
    public class PublicacaoService
    {
        private readonly PublicacaoRepository _repository;
        private readonly PesquisadorRepository _pesquisadorRepository;
        private readonly ProjetoRepository _projetoRepository;

        public PublicacaoService(PublicacaoRepository repository,
            PesquisadorRepository pesquisadorRepository,
            ProjetoRepository projetoRepository)
        {
            _repository = repository;
            _pesquisadorRepository = pesquisadorRepository;
            _projetoRepository = projetoRepository;
        }

        public List<PublicacaoResumo> Listar(FiltroPublicacoes filtro)
        {
            return _repository.Listar(filtro);
        }

        public List<PublicacaoResumo> ListarRecentes(int limite)
        {
            return _repository.ListarRecentes(limite);
        }

        public List<PublicacaoResumo> ListarGerenciaveis(UsuarioSessao usuario, string termo = null)
        {
            var lista = _repository.Listar(new FiltroPublicacoes { Termo = termo });
            if (usuario == null || usuario.EhAdministrador) return lista;
            if (!usuario.EhCoordenador || !usuario.PesquisadorId.HasValue) return new List<PublicacaoResumo>();

            var idsProjetos = _projetoRepository.ListarDoCoordenador(usuario.PesquisadorId.Value)
                .Select(p => p.ProjetoId).ToList();
            return lista.Where(p => p.ProjetoId.HasValue && idsProjetos.Contains(p.ProjetoId.Value)).ToList();
        }

        public PublicacaoResumo ObterResumo(int id)
        {
            return _repository.ObterResumo(id);
        }

        public Publicacao ObterEntidade(int id)
        {
            return _repository.ObterEntidade(id);
        }

        public List<int> ListarAnos()
        {
            return _repository.ListarAnos();
        }

        public int Salvar(Publicacao publicacao, UsuarioSessao usuario)
        {
            if (publicacao == null)
                throw new ValidacaoException("Dados da publicação não informados.");
            if (usuario == null || !usuario.PodeGerenciarProjetos)
                throw new RegraNegocioException("Você não possui permissão para gerenciar publicações.");

            var erros = new List<string>();
            Validacao.Coletar(erros, () => publicacao.Validar());
            Validacao.Coletar(erros, () => publicacao.Titulo = Validacao.TextoObrigatorio(publicacao.Titulo, "título", 5, 300));
            Validacao.Coletar(erros, () => publicacao.Veiculo = Validacao.TextoObrigatorio(publicacao.Veiculo, "veículo", 2, 200));
            Validacao.Coletar(erros, () => publicacao.Doi = string.IsNullOrWhiteSpace(publicacao.Doi) ? null
                : Validacao.TextoObrigatorio(publicacao.Doi, "DOI", 4, 150));
            Validacao.Coletar(erros, () => publicacao.LinkAcesso = Validacao.UrlOpcional(publicacao.LinkAcesso, "link de acesso"));
            Validacao.LancarSeHouver(erros);

            if (publicacao.Autores == null || publicacao.Autores.Count == 0)
                throw new ValidacaoException("Informe ao menos um autor pesquisador para a publicação.");
            if (publicacao.Autores.Select(a => a.PesquisadorId).Distinct().Count() != publicacao.Autores.Count)
                throw new ValidacaoException("Um mesmo pesquisador não pode ser informado duas vezes como autor.");
            foreach (var autor in publicacao.Autores)
            {
                if (_pesquisadorRepository.ObterResumo(autor.PesquisadorId) == null)
                    throw new ValidacaoException("Um dos autores selecionados não está cadastrado como pesquisador.");
            }

            if (!usuario.EhAdministrador)
            {
                var projetoId = publicacao.ProjetoId ?? (publicacao.PublicacaoId > 0
                    ? _repository.ObterEntidade(publicacao.PublicacaoId).ProjetoId : null);
                if (!projetoId.HasValue || !usuario.PesquisadorId.HasValue)
                    throw new RegraNegocioException("Você só pode gerenciar publicações vinculadas a projetos sob sua coordenação.");

                var projeto = _projetoRepository.ObterResumo(projetoId.Value);
                if (projeto == null || projeto.CoordenadorId != usuario.PesquisadorId.Value)
                    throw new RegraNegocioException("Você só pode gerenciar publicações vinculadas a projetos sob sua coordenação.");
                publicacao.ProjetoId = projetoId;
            }

            if (publicacao.PublicacaoId > 0)
            {
                _repository.Atualizar(publicacao);
                return publicacao.PublicacaoId;
            }
            return _repository.Inserir(publicacao);
        }

        public void Excluir(int publicacaoId, UsuarioSessao usuario)
        {
            var publicacao = _repository.ObterEntidade(publicacaoId);
            if (publicacao == null) throw new ValidacaoException("Publicação não encontrada.");

            if (!usuario.EhAdministrador)
            {
                if (!publicacao.ProjetoId.HasValue)
                    throw new RegraNegocioException("Somente o administrador pode excluir publicações sem projeto vinculado.");
                var projeto = _projetoRepository.ObterResumo(publicacao.ProjetoId.Value);
                if (projeto == null || projeto.CoordenadorId != usuario.PesquisadorId)
                    throw new RegraNegocioException("Você só pode excluir publicações de projetos sob sua coordenação.");
            }

            _repository.Excluir(publicacaoId);
        }

        /* ------------------------------- Destaques ------------------------------- */

        public List<DestaqueAdminResumo> ListarDestaques(int? pesquisadorId = null)
        {
            return _repository.ListarDestaques(pesquisadorId);
        }

        public List<PublicacaoResumo> ListarPublicacoesDoPesquisador(int pesquisadorId)
        {
            return _pesquisadorRepository.ListarPublicacoes(pesquisadorId);
        }

        public void AdicionarDestaque(int pesquisadorId, int publicacaoId, int ordem, UsuarioSessao usuario)
        {
            AutorizarDestaque(pesquisadorId, usuario);

            if (ordem < 1 || ordem > 5)
                throw new ValidacaoException("A posição do destaque deve estar entre 1 e 5.");
            if (_repository.ContarDestaques(pesquisadorId) >= 5)
                throw new ValidacaoException("Este pesquisador já possui 5 publicações em destaque. Remova uma antes de adicionar outra (RN05).");
            if (!_repository.PesquisadorEhAutor(pesquisadorId, publicacaoId))
                throw new ValidacaoException("Só é possível destacar uma publicação da qual o pesquisador seja autor.");

            var existentes = _repository.ListarDestaques(pesquisadorId);
            if (existentes.Any(d => d.Ordem == ordem))
                throw new ValidacaoException("A posição " + ordem + " já está ocupada. Escolha outra posição ou remova o destaque atual.");

            _repository.InserirDestaque(pesquisadorId, publicacaoId, ordem);
        }

        public void RemoverDestaque(int destaqueId, UsuarioSessao usuario)
        {
            var destaque = _repository.ListarDestaques().FirstOrDefault(d => d.DestaqueId == destaqueId);
            if (destaque == null) throw new ValidacaoException("Destaque não encontrado.");
            AutorizarDestaque(destaque.PesquisadorId, usuario);
            _repository.ExcluirDestaque(destaqueId);
        }

        private void AutorizarDestaque(int pesquisadorId, UsuarioSessao usuario)
        {
            if (usuario == null) throw new RegraNegocioException("Sessão expirada.");
            if (usuario.EhAdministrador) return;
            if (usuario.EhCoordenador && usuario.PesquisadorId == pesquisadorId) return;
            throw new RegraNegocioException("Você só pode gerenciar os destaques do seu próprio perfil.");
        }
    }
}
