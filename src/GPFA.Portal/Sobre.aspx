<%@ Page Title="Sobre o GPFA" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Sobre.aspx.cs" Inherits="GPFA.Portal.Sobre" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-hero">
        <div class="container-gpfa">
            <div class="eyebrow">Institucional · O grupo</div>
            <h1>Pequenos times, perguntas gigantes</h1>
            <p>O Grupo de Pesquisa sobre Formigas Atômicas (GPFA) nasceu da convicção de que os melhores problemas científicos exigem olhares de áreas diferentes — e de que ciência pública se faz com transparência.</p>
            <div class="breadcrumb"><a href="<%= ResolveUrl("~/Default.aspx") %>">Início</a> / O grupo</div>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="row g-4 align-items-start">
                <div class="col-lg-7">
                    <div class="section-label">Nossa história</div>
                    <p class="bio-text">
                        O GPFA reúne pesquisadores e estudantes da Universidade Federal de Minas Gerais em torno de
                        materiais quânticos, nanofotônica, simulação computacional, materiais funcionais e sensores ambientais.
                        O nome é uma brincadeira séria: investigamos fenômenos que só aparecem quando olhamos para escalas
                        minúsculas, mas acreditamos que a ciência só completa seu ciclo quando chega às pessoas.
                    </p>
                    <p class="bio-text">
                        Nosso laboratório funciona como uma rede. Cada projeto tem um único coordenador responsável perante a
                        instituição, mas se beneficia da participação de múltiplos pesquisadores e estudantes. Todo estudante
                        possui um orientador principal cadastrado e vínculo com pelo menos um projeto — regras que este portal
                        torna visíveis e auditáveis.
                    </p>
                    <div class="row g-3 mt-3">
                        <div class="col-sm-6">
                            <div class="panel">
                                <div class="panel-title">Missão</div>
                                <p class="mb-0 small">Produzir conhecimento de fronteira e formar pessoas capazes de atravessar fronteiras disciplinares.</p>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="panel">
                                <div class="panel-title">Compromisso</div>
                                <p class="mb-0 small">Transparência sobre pessoas, projetos e recursos; acesso aberto à produção científica sempre que possível.</p>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-5">
                    <div class="panel" style="background:var(--brand-950);color:#c7d6df;border:0">
                        <div class="eyebrow">O grupo em números</div>
                        <div class="row g-3 mt-2">
                            <div class="col-6">
                                <div class="stat-label" style="color:#8fa9b8">Pesquisadores</div>
                                <div class="stat-value" style="color:#fff"><asp:Literal ID="litPesquisadores" runat="server" /></div>
                            </div>
                            <div class="col-6">
                                <div class="stat-label" style="color:#8fa9b8">Estudantes</div>
                                <div class="stat-value" style="color:#fff"><asp:Literal ID="litEstudantes" runat="server" /></div>
                            </div>
                            <div class="col-6">
                                <div class="stat-label" style="color:#8fa9b8">Projetos</div>
                                <div class="stat-value" style="color:#fff"><asp:Literal ID="litProjetos" runat="server" /></div>
                            </div>
                            <div class="col-6">
                                <div class="stat-label" style="color:#8fa9b8">Publicações</div>
                                <div class="stat-value" style="color:#fff"><asp:Literal ID="litPublicacoes" runat="server" /></div>
                            </div>
                        </div>
                        <div class="divider" style="background:rgba(255,255,255,.12)"></div>
                        <div class="tiny" style="color:#9fb5c2">Verba captada em projetos</div>
                        <div style="font-family:var(--font-serif);font-size:1.8rem;color:var(--accent)"><asp:Literal ID="litVerba" runat="server" /></div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="section section-alt" id="linhas">
        <div class="container-gpfa">
            <div class="section-head">
                <div>
                    <div class="section-kicker">Linhas de pesquisa</div>
                    <h2 class="section-title">Onde colocamos nossa curiosidade</h2>
                </div>
            </div>
            <div class="card-grid grid-2">
                <div class="panel">
                    <div class="d-flex gap-3">
                        <svg class="icon icon-lg" style="color:var(--teal);flex-shrink:0" aria-hidden="true"><use href="#i-layers"/></svg>
                        <div>
                            <h3 style="font-size:1.15rem">Materiais Quânticos</h3>
                            <p class="small muted mb-0">Heteroestruturas bidimensionais, transporte eletrônico e dispositivos de baixo consumo. Buscamos entender como a desordem e as interfaces afetam propriedades emergentes.</p>
                        </div>
                    </div>
                </div>
                <div class="panel">
                    <div class="d-flex gap-3">
                        <svg class="icon icon-lg" style="color:var(--accent);flex-shrink:0" aria-hidden="true"><use href="#i-flask"/></svg>
                        <div>
                            <h3 style="font-size:1.15rem">Nanofotônica</h3>
                            <p class="small muted mb-0">Microcavidades, metamateriais e sensores ópticos. O objetivo é detectar sinais minúsculos — biomarcadores, gases — com dispositivos portáteis.</p>
                        </div>
                    </div>
                </div>
                <div class="panel">
                    <div class="d-flex gap-3">
                        <svg class="icon icon-lg" style="color:var(--brand-500);flex-shrink:0" aria-hidden="true"><use href="#i-chart"/></svg>
                        <div>
                            <h3 style="font-size:1.15rem">Simulação Computacional</h3>
                            <p class="small muted mb-0">Modelagem multiescala, métodos numéricos de alto desempenho e aprendizado de máquina aplicado a materiais e sistemas quânticos.</p>
                        </div>
                    </div>
                </div>
                <div class="panel">
                    <div class="d-flex gap-3">
                        <svg class="icon icon-lg" style="color:#4f9d69;flex-shrink:0" aria-hidden="true"><use href="#i-pin"/></svg>
                        <div>
                            <h3 style="font-size:1.15rem">Sensores Ambientais e Materiais Funcionais</h3>
                            <p class="small muted mb-0">Redes abertas de monitoramento de rios, materiais para remediação de água e ciência cidadã com comunidades ribeirinhas.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="section-head">
                <div>
                    <div class="section-kicker">Como o portal funciona</div>
                    <h2 class="section-title">Transparência por arquitetura</h2>
                    <p class="section-desc">Este não é um site estático: é um sistema em camadas, com banco relacional e regras de negócio verificáveis.</p>
                </div>
            </div>
            <div class="card-grid grid-4">
                <div class="panel">
                    <div class="panel-title">Apresentação</div>
                    <h3 style="font-size:1.05rem">ASP.NET Web Forms</h3>
                    <p class="small muted mb-0">Master Pages, controles de servidor, Data Binding, ciclo de vida e Session.</p>
                </div>
                <div class="panel">
                    <div class="panel-title">Negócio</div>
                    <h3 style="font-size:1.05rem">Regras centralizadas</h3>
                    <p class="small muted mb-0">Validações de vínculo, coordenação única, orientação e limite de destaques.</p>
                </div>
                <div class="panel">
                    <div class="panel-title">Dados</div>
                    <h3 style="font-size:1.05rem">SQL Server</h3>
                    <p class="small muted mb-0">Chaves estrangeiras, restrições, triggers, views e consultas parametrizadas.</p>
                </div>
                <div class="panel">
                    <div class="panel-title">Qualidade</div>
                    <h3 style="font-size:1.05rem">Testes e documentação</h3>
                    <p class="small muted mb-0">Casos de teste, documentação de requisitos, MER/DER e histórico de decisões.</p>
                </div>
            </div>
        </div>
    </section>

    <section class="section section-alt" id="contato">
        <div class="container-gpfa">
            <div class="row g-5">
                <div class="col-lg-6">
                    <div class="section-kicker">Contato</div>
                    <h2 class="section-title">Vamos conversar?</h2>
                    <p class="section-desc">Para parcerias de pesquisa, visitas, divulgação científica ou interesse em orientação, escreva para a coordenação do grupo.</p>
                    <div class="panel mt-4">
                        <div class="data-list">
                            <div class="data-row">
                                <div class="data-row-main">
                                    <svg class="icon" style="color:var(--brand-500)" aria-hidden="true"><use href="#i-mail"/></svg>
                                    <div><p class="data-row-title">E-mail</p><div class="data-row-sub">contato@gpfa.ufmg.br</div></div>
                                </div>
                                <a class="btn-link-gpfa" href="mailto:contato@gpfa.ufmg.br">Escrever →</a>
                            </div>
                            <div class="data-row">
                                <div class="data-row-main">
                                    <svg class="icon" style="color:var(--brand-500)" aria-hidden="true"><use href="#i-pin"/></svg>
                                    <div><p class="data-row-title">Endereço</p><div class="data-row-sub">Departamento de Física · UFMG · Belo Horizonte, MG</div></div>
                                </div>
                            </div>
                            <div class="data-row">
                                <div class="data-row-main">
                                    <svg class="icon" style="color:var(--brand-500)" aria-hidden="true"><use href="#i-users"/></svg>
                                    <div><p class="data-row-title">Coordenação</p><div class="data-row-sub">Profa. Dra. Helena Marques Ferreira</div></div>
                                </div>
                                <a class="btn-link-gpfa" href="<%= ResolveUrl("~/Pesquisador.aspx") + "?id=1" %>">Perfil →</a>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="panel" style="height:100%;background:linear-gradient(135deg,#fff,#f3f8f9)">
                        <div class="panel-title">Para estudantes</div>
                        <h3 style="font-size:1.4rem">Quer fazer ciência com a gente?</h3>
                        <p class="small muted">Estudantes de graduação e pós-graduação interessados em iniciação científica, mestrado ou doutorado podem procurar diretamente os pesquisadores de sua área. Todas as vagas e vínculos são formalizados com um orientador principal e um projeto de pesquisa.</p>
                        <a class="btn-gpfa btn-brand mt-2" href="<%= ResolveUrl("~/Pesquisadores.aspx") %>">Encontrar um orientador</a>
                        <div class="divider"></div>
                        <p class="tiny muted mb-0">Este portal é um projeto acadêmico desenvolvido para a disciplina de Programação Web, com dados de demonstração fictícios.</p>
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
