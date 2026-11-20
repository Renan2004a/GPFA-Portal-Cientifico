<%@ Page Title="Visibilidade científica e gestão de projetos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GPFA.Portal.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <section class="hero">
        <div class="container-gpfa hero-inner">
            <div class="hero-copy">
                <div class="eyebrow">Grupo de Pesquisa · UFMG</div>
                <h1 class="hero-title">Ciência que conecta átomos, pessoas e <em>impacto</em>.</h1>
                <p class="hero-lead">
                    Somos o GPFA — um laboratório multidisciplinar que investiga materiais quânticos,
                    nanofotônica, simulação computacional e sensores ambientais. Este portal reúne
                    nossa produção científica, nossos projetos e as pessoas que constroem essa história.
                </p>
                <div class="hero-actions">
                    <a class="btn-gpfa btn-accent btn-lg" href="<%= ResolveUrl("~/Projetos.aspx") %>">
                        Explorar projetos
                        <svg class="icon" aria-hidden="true"><use href="#i-arrow-right"/></svg>
                    </a>
                    <a class="btn-gpfa btn-light-outline btn-lg" href="<%= ResolveUrl("~/Pesquisadores.aspx") %>">
                        Conhecer a equipe
                    </a>
                </div>
                <div class="hero-note">
                    <span><svg class="icon icon-sm" aria-hidden="true"><use href="#i-calendar"/></svg> Produção 2019–2026</span>
                    <span><svg class="icon icon-sm" aria-hidden="true"><use href="#i-coins"/></svg> <asp:Literal ID="litVerbaHero" runat="server" /> captados</span>
                    <span><svg class="icon icon-sm" aria-hidden="true"><use href="#i-pin"/></svg> Belo Horizonte · MG</span>
                </div>
            </div>
            <div class="hero-art" aria-hidden="true">
                <div class="orbit-stage">
                    <div class="orbit-ring"></div>
                    <div class="orbit-ring r2"></div>
                    <div class="orbit-ring r3"></div>
                    <span class="orbit-dot d1"></span>
                    <span class="orbit-dot d2"></span>
                    <span class="orbit-dot d3"></span>
                    <div class="orbit-core">
                        <svg viewBox="0 0 64 64" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round">
                            <ellipse cx="32" cy="32" rx="27" ry="11" transform="rotate(-28 32 32)" opacity=".7"/>
                            <ellipse cx="32" cy="32" rx="27" ry="11" transform="rotate(34 32 32)" opacity=".45"/>
                            <circle cx="32" cy="26" r="4" fill="currentColor" stroke="none"/>
                            <circle cx="32" cy="33" r="5.5" fill="currentColor" stroke="none" opacity=".85"/>
                            <circle cx="32" cy="41" r="7" fill="currentColor" stroke="none" opacity=".7"/>
                            <path d="M27 24l-6-6M37 24l6-6M25 33l-8 3M39 33l8 3M26 44l-6 7M38 44l6 7" opacity=".8"/>
                        </svg>
                    </div>
                    <div class="orbit-caption">formigas atômicas · desde 2018</div>
                </div>
            </div>
        </div>
    </section>

    <section class="stats-band" aria-label="Indicadores do grupo">
        <div class="container-gpfa">
            <div class="stats-grid">
                <div class="stat">
                    <span class="stat-label">Pesquisadores</span>
                    <div class="stat-value"><asp:Literal ID="litPesquisadores" runat="server" /></div>
                    <div class="stat-note">docentes e pós-doutores</div>
                </div>
                <div class="stat">
                    <span class="stat-label">Estudantes</span>
                    <div class="stat-value"><asp:Literal ID="litEstudantes" runat="server" /></div>
                    <div class="stat-note"><asp:Literal ID="litEstudantesAtivos" runat="server" /> em atividade</div>
                </div>
                <div class="stat">
                    <span class="stat-label">Projetos de pesquisa</span>
                    <div class="stat-value"><asp:Literal ID="litProjetos" runat="server" /></div>
                    <div class="stat-note"><asp:Literal ID="litProjetosAndamento" runat="server" /> em andamento</div>
                </div>
                <div class="stat">
                    <span class="stat-label">Publicações</span>
                    <div class="stat-value"><asp:Literal ID="litPublicacoes" runat="server" /></div>
                    <div class="stat-note">artigos e capítulos indexados</div>
                </div>
            </div>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="section-head">
                <div>
                    <div class="section-kicker">Projetos em destaque</div>
                    <h2 class="section-title">Perguntas grandes, equipes diversas</h2>
                    <p class="section-desc">Cada projeto reúne pesquisadores, estudantes e parceiros em torno de um problema científico concreto.</p>
                </div>
                <a class="link-arrow" href="<%= ResolveUrl("~/Projetos.aspx") %>">Ver todos os projetos
                    <svg class="icon icon-sm" aria-hidden="true"><use href="#i-arrow-right"/></svg>
                </a>
            </div>

            <div class="card-grid grid-3">
                <asp:Repeater ID="rptProjetos" runat="server">
                    <ItemTemplate>
                        <article class="project-card">
                            <div class="project-media">
                                <img src='<%# UrlAttr(Eval("ImagemUrl")) %>' alt="" loading="lazy" />
                                <span class='status-pill <%# Eval("SituacaoCss") %>'><%#: Eval("Situacao") %></span>
                            </div>
                            <div class="project-body">
                                <div class="project-line"><%#: Eval("LinhaPesquisa") %></div>
                                <h3 class="project-title">
                                    <a href='<%# ResolveUrl("~/Projeto.aspx") + "?id=" + Eval("ProjetoId") %>'><%#: Eval("Titulo") %></a>
                                </h3>
                                <p class="project-summary"><%#: Eval("Resumo") %></p>
                                <div class="project-meta">
                                    <span class="project-coord">
                                        <%# AvatarHtml(Eval("CoordenadorFotoUrl"), Eval("CoordenadorNome"), "avatar avatar-sm") %>
                                        <%#: Eval("CoordenadorNome") %>
                                    </span>
                                    <span class="meta-item"><%#: Eval("QtdPesquisadores") %> pesq. · <%#: Eval("QtdEstudantes") %> est.</span>
                                </div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <asp:Panel ID="pnlSemProjetos" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><svg class="icon icon-lg" aria-hidden="true"><use href="#i-flask"/></svg></div>
                <h3>Nenhum projeto cadastrado</h3>
                <p>Assim que os primeiros projetos forem publicados, eles aparecerão aqui.</p>
            </asp:Panel>
        </div>
    </section>

    <section class="section section-alt" id="linhas">
        <div class="container-gpfa">
            <div class="section-head">
                <div>
                    <div class="section-kicker">Linhas de pesquisa</div>
                    <h2 class="section-title">Quatro frentes, uma mesma curiosidade</h2>
                    <p class="section-desc">Do comportamento de elétrons em interfaces à qualidade da água de um rio: a ciência acontece nas conexões.</p>
                </div>
            </div>
            <div class="card-grid grid-4">
                <a class="panel" style="text-decoration:none" href='<%= ResolveUrl("~/Projetos.aspx") + "?linha=Materiais+Qu%C3%A2nticos" %>'>
                    <svg class="icon icon-lg" style="color:var(--teal)" aria-hidden="true"><use href="#i-layers"/></svg>
                    <h3 class="mt-2" style="font-size:1.1rem">Materiais Quânticos</h3>
                    <p class="small muted mb-0">Heteroestruturas 2D, transporte eletrônico e dispositivos de baixo consumo.</p>
                </a>
                <a class="panel" style="text-decoration:none" href='<%= ResolveUrl("~/Projetos.aspx") + "?linha=Nanofot%C3%B4nica" %>'>
                    <svg class="icon icon-lg" style="color:var(--accent)" aria-hidden="true"><use href="#i-flask"/></svg>
                    <h3 class="mt-2" style="font-size:1.1rem">Nanofotônica</h3>
                    <p class="small muted mb-0">Microcavidades, metamateriais e sensores ópticos de alta sensibilidade.</p>
                </a>
                <a class="panel" style="text-decoration:none" href='<%= ResolveUrl("~/Projetos.aspx") + "?linha=Simula%C3%A7%C3%A3o+Computacional" %>'>
                    <svg class="icon icon-lg" style="color:var(--brand-500)" aria-hidden="true"><use href="#i-chart"/></svg>
                    <h3 class="mt-2" style="font-size:1.1rem">Simulação Computacional</h3>
                    <p class="small muted mb-0">Modelagem multiescala, aprendizado de máquina e visualização científica.</p>
                </a>
                <a class="panel" style="text-decoration:none" href='<%= ResolveUrl("~/Projetos.aspx") + "?linha=Sensores+Ambientais" %>'>
                    <svg class="icon icon-lg" style="color:#4f9d69" aria-hidden="true"><use href="#i-pin"/></svg>
                    <h3 class="mt-2" style="font-size:1.1rem">Sensores Ambientais</h3>
                    <p class="small muted mb-0">Redes abertas de monitoramento de rios, dados públicos e ciência cidadã.</p>
                </a>
            </div>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="section-head">
                <div>
                    <div class="section-kicker">Pessoas</div>
                    <h2 class="section-title">Quem faz o GPFA acontecer</h2>
                    <p class="section-desc">Pesquisadores com trajetórias complementares e estudantes em formação — do técnico ao pós-doutorado.</p>
                </div>
                <a class="link-arrow" href="<%= ResolveUrl("~/Pesquisadores.aspx") %>">Diretório completo
                    <svg class="icon icon-sm" aria-hidden="true"><use href="#i-arrow-right"/></svg>
                </a>
            </div>
            <div class="card-grid grid-3">
                <asp:Repeater ID="rptPesquisadores" runat="server">
                    <ItemTemplate>
                        <article class="person-card">
                            <div class="person-head">
                                <%# AvatarHtml(Eval("FotoUrl"), Eval("NomeCompleto")) %>
                                <div>
                                    <h3 class="person-name">
                                        <a href='<%# ResolveUrl("~/Pesquisador.aspx") + "?id=" + Eval("PesquisadorId") %>'><%#: Eval("NomeCompleto") %></a>
                                    </h3>
                                    <div class="person-role"><%#: Eval("TitulacaoExibicao") %><br /><%#: Eval("Departamento") %></div>
                                </div>
                            </div>
                            <span class="person-area"><%#: Eval("AreaAtuacao") %></span>
                            <p class="person-bio"><%#: Eval("MiniBiografia") %></p>
                            <div class="person-foot">
                                <div class="person-stats">
                                    <span><strong><%#: Eval("QtdProjetos") %></strong> projetos</span>
                                    <span><strong><%#: Eval("QtdOrientandosAtuais") %></strong> orientandos</span>
                                </div>
                                <a class="btn-link-gpfa" href='<%# ResolveUrl("~/Pesquisador.aspx") + "?id=" + Eval("PesquisadorId") %>'>Perfil →</a>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <section class="section section-alt">
        <div class="container-gpfa">
            <div class="section-head">
                <div>
                    <div class="section-kicker">Produção científica</div>
                    <h2 class="section-title">Conhecimento que circula</h2>
                    <p class="section-desc">Publicações recentes do grupo, com acesso aberto sempre que possível.</p>
                </div>
                <a class="link-arrow" href="<%= ResolveUrl("~/Publicacoes.aspx") %>">Todas as publicações
                    <svg class="icon icon-sm" aria-hidden="true"><use href="#i-arrow-right"/></svg>
                </a>
            </div>
            <div class="pub-list">
                <asp:Repeater ID="rptPublicacoes" runat="server">
                    <ItemTemplate>
                        <article class="pub-item">
                            <div class="pub-rank"><%#: Eval("Ano") %></div>
                            <div>
                                <h3 class="pub-title">
                                    <a href='<%# UrlAttr(Eval("UrlAcesso")) %>' target="_blank" rel="noopener noreferrer"><%#: Eval("Titulo") %></a>
                                </h3>
                                <div class="pub-meta">
                                    <span class="vehicle"><%#: Eval("Veiculo") %></span> · <%#: Eval("Tipo") %>
                                    <div class="pub-authors"><%#: Eval("Autores") %></div>
                                </div>
                            </div>
                            <a class="pub-year" href='<%# UrlAttr(Eval("UrlAcesso")) %>' target="_blank" rel="noopener noreferrer">Acessar</a>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <asp:Panel ID="pnlSemPublicacoes" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><svg class="icon icon-lg" aria-hidden="true"><use href="#i-book"/></svg></div>
                <h3>Nenhuma publicação cadastrada</h3>
                <p>A produção científica do grupo será exibida aqui.</p>
            </asp:Panel>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="panel" style="background:linear-gradient(120deg,var(--brand-950),var(--brand-800));border:0;color:#dbe8ef;padding:3rem">
                <div class="d-flex flex-wrap align-items-center justify-content-between gap-3">
                    <div style="max-width:62ch">
                        <div class="eyebrow">Colaboração</div>
                        <h2 style="color:#fff;font-size:clamp(1.8rem,3vw,2.4rem);margin-bottom:.6rem">Tem uma pergunta que só a ciência em rede responde?</h2>
                        <p style="color:#b9cbd6;margin:0">Buscamos parcerias com instituições, empresas e comunidades para transformar pesquisa em impacto real. Estudantes interessados também podem conversar com nossos orientadores.</p>
                    </div>
                    <div class="d-flex gap-2 flex-wrap">
                        <a class="btn-gpfa btn-accent" href="<%= ResolveUrl("~/Sobre.aspx#contato") %>">Falar com o grupo</a>
                        <a class="btn-gpfa btn-light-outline" href="<%= ResolveUrl("~/Estudantes.aspx") %>">Ver estudantes</a>
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
