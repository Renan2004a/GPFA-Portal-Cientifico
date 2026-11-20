using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GPFA.Models;

namespace GPFA.Business
{
    /// <summary>
    /// Validações compartilhadas pelas camadas de negócio. As mensagens são
    /// voltadas ao usuário e nunca contêm detalhes internos.
    /// </summary>
    public static class Validacao
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex LoginRegex = new Regex(
            @"^[A-Za-z0-9._-]{3,30}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static void Requerer(bool condicao, string mensagem)
        {
            if (!condicao) throw new ValidacaoException(mensagem);
        }

        public static string TextoObrigatorio(string valor, string campo, int minimo = 1, int maximo = 500)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ValidacaoException("Informe " + campo + ".");
            var limpo = valor.Trim();
            if (limpo.Length < minimo)
                throw new ValidacaoException("O campo " + campo + " deve ter ao menos " + minimo + " caracteres.");
            if (limpo.Length > maximo)
                throw new ValidacaoException("O campo " + campo + " deve ter no máximo " + maximo + " caracteres.");
            return limpo;
        }

        public static string Email(string valor)
        {
            var limpo = TextoObrigatorio(valor, "e-mail", 5, 150);
            if (!EmailRegex.IsMatch(limpo))
                throw new ValidacaoException("Informe um e-mail válido.");
            return limpo.ToLowerInvariant();
        }

        public static string Login(string valor)
        {
            var limpo = TextoObrigatorio(valor, "login", 3, 30);
            if (!LoginRegex.IsMatch(limpo))
                throw new ValidacaoException(
                    "O login deve ter de 3 a 30 caracteres e conter apenas letras, números, ponto, hífen ou sublinhado.");
            return limpo.ToLowerInvariant();
        }

        /// <summary>Valida URL opcional, aceitando apenas http/https.</summary>
        public static string UrlOpcional(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            var limpo = valor.Trim();
            if (limpo.Length > 400)
                throw new ValidacaoException("O endereço de " + campo + " é muito longo.");

            Uri uri;
            if (!Uri.TryCreate(limpo, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ValidacaoException("O endereço de " + campo + " deve começar com http:// ou https://.");
            }
            return limpo;
        }

        public static string UrlOpcionalOuRelativa(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;
            var limpo = valor.Trim();
            if (limpo.StartsWith("~/") || limpo.StartsWith("/"))
                return limpo;
            return UrlOpcional(limpo, campo);
        }

        public static DateTime DataObrigatoria(DateTime? valor, string campo)
        {
            if (!valor.HasValue || valor.Value == DateTime.MinValue)
                throw new ValidacaoException("Informe " + campo + ".");
            return valor.Value.Date;
        }

        public static DateTime DataNaoFutura(DateTime? valor, string campo)
        {
            var data = DataObrigatoria(valor, campo);
            if (data > DateTime.Today)
                throw new ValidacaoException("A " + campo + " não pode ser uma data futura.");
            return data;
        }

        public static int IdObrigatorio(int valor, string campo)
        {
            if (valor <= 0)
                throw new ValidacaoException("Selecione " + campo + ".");
            return valor;
        }

        public static void Coletar(List<string> erros, Action acao)
        {
            try { acao(); }
            catch (ValidacaoException ex) { erros.AddRange(ex.Erros); }
        }

        public static void LancarSeHouver(List<string> erros)
        {
            if (erros != null && erros.Count > 0)
                throw new ValidacaoException(erros);
        }
    }
}
