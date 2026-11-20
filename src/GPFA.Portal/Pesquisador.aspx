<%@ Page Title="Perfil do pesquisador" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pesquisador.aspx.cs" Inherits="GPFA.Portal.Pesquisador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlNaoEncontrado" runat="server" Visible="false">
        <section class="page-hero">
            <div class="container-gpfa">
                <div class="eyebrow">Pessoas · Perfil</div>
                <h1>Pesquisador não encontrado</h1>
                <p>O perfil solicitado não existe ou foi removido. O diretório está sempre atualizado.</p>
                <div class="mt-3"><a class="btn-gpfa btn-accent" href="<%= ResolveUrl("~/Pesquisadores.aspx") %>">Voltar ao diretório</a></div>
            </div>
        </section>
    </asp:Panel>

    <asp:Panel ID="pnlPerfil" runat="server" Visible="false">
        <section class="profile-cover">
            <div class="container-gpfa">
                <div class="profile-header">
                    <div class="profile-photo">
                        <asp:Literal ID="litFoto" runat="server" />
                    </div>
                    <div class="profile-identity">
                        <div class="role"><asp:Literal ID="litArea" runat="server" /></div>
                        <h1><asp:Literal ID="litNome" runat="server" /></h1>
                        <div class="formation"><asp:Literal ID="litFormacao" runat="server" /></div>
                    </div>
                    <div class="profile-links">
                        <asp:HyperLink ID="lnkScholar" runat="server" Target="_blank" rel="noopener noreferrer">
                            <svg class="icon icon-sm" aria-hidden="true"><use href="#i-arrow-up-right"/></svg> Google Scholar
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkOrcid" runat="server" Target="_blank" rel="noopener noreferrer">
                            <svg class="icon icon-sm" aria-hidden="true"><use href="#i-arrow-up-right"/></svg> ORCID
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkLattes" runat="server" Target="_blank" rel="noopener noreferrer">
                            <svg class="icon icon-sm" aria-hidden="true"><use href="#i-file"/></svg> Lattes
                        </asp:HyperLink>
                        <asp:HyperLink ID="lnkEmail" runat="server" CssClass="text-decoration-none">
                            <svg class="icon icon-sm" aria-hidden="true"><use href="#i-mail"/></svg> E-mail
                        </asp:HyperLink>
                    </div>
                </div>
            </div>
        </section>

        <div class="container-gpfa">
            <div class="profile-grid">
                <div class="profile-main">
                    <section>
                        <h2 class="section-label">Sobre</h2>
                        <p class="bio-text"><asp:Literal ID="litBio" runat="server" /></p>
                        <div class="tag-list mt-2">
                            <span class="tag"><asp:Literal ID="litLinha" runat="server" /></span>
                            <span class="tag"><asp:Literal ID="litDepartamento" runat="server" /></span>
                        </div>
                    </section>

                    <section>
                        <h2 class="section-label">Cinco publicações em destaque</h2>
                        <div class="pub-list pub-highlight-list">
                            <asp:Repeater ID="rptDestaques" runat="server">
                                <ItemTemplate>
                                    <article class="pub-item">
                                        <div class="pub-rank"><%# Container.ItemIndex + 1 %></div>
                                        <div>
                                            <h3 class="pub-title">
                                                <a href='<%# UrlAttr(Eval("UrlAcesso")) %>' target="_blank" rel="noopener noreferrer"><%#: Eval("Titulo") %></a>
                                            </h3>
                                            <div class="pub-meta"><span class="vehicle"><%#: Eval("Veiculo") %></span> · <%#: Eval("Ano") %></div>
                                            <div class="pub-authors"><%#: Eval("Autores") %></div>
                                        </div>
                                        <span class="pub-year"><%#: Eval("Ano") %></span>
                                    </article>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <asp:Panel ID="pnlSemDestaques" runat="server" Visible="false" CssClass="empty-state" style="padding:2rem">
                            <p class="mb-0">Este pesquisador ainda não selecionou publicações em destaque.</p>
                        </asp:Panel>
                    </section>

                    <section>
                        <h2 class="section-label">Projetos vinculados</h2>
                        <div class="card-grid grid-2">
                            <asp:Repeater ID="rptProjetos" runat="server">
                                <ItemTemplate>
                                    <article class="project-card">
                                        <div class="project-body">
                                            <div class="d-flex justify-content-between align-items-start gap-2">
                                                <div class="project-line"><%#: Eval("LinhaPesquisa") %></div>
                                                <span class='status-pill <%# Eval("SituacaoCss") %>'><%#: Eval("Situacao") %></span>
                                            </div>
                                            <h3 class="project-title" style="font-size:1.05rem">
                                                <a href='<%# ResolveUrl("~/Projeto.aspx") + "?id=" + Eval("ProjetoId") %>'><%#: Eval("Titulo") %></a>
                                            </h3>
                                            <div class="project-meta" style="padding-top:.7rem">
                                                <span class="meta-item"><%#: Eval("PeriodoFormatado") %></span>
                                                <span class="meta-item"><%#: Eval("QtdPesquisadores") %> pesquisadores</span>
                                            </div>
                                        </div>
                                    </article>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <asp:Panel ID="pnlSemProjetos" runat="server" Visible="false" CssClass="empty-state" style="padding:2rem">
                            <p class="mb-0">Nenhum projeto vinculado no momento.</p>
                        </asp:Panel>
                    </section>

                    <section>
                        <h2 class="section-label">Orientandos atuais</h2>
                        <div class="panel">
                            <div class="timeline">
                                <asp:Repeater ID="rptOrientandosAtuais" runat="server">
                                    <ItemTemplate>
                                        <div class="timeline-item current">
                                            <div class="d-flex align-items-center gap-2">
                                                <%# AvatarHtml(Eval("EstudanteFotoUrl"), Eval("EstudanteNome"), "avatar avatar-sm") %>
                                                <div>
                                                    <p class="timeline-title"><%#: Eval("EstudanteNome") %></p>
                                                    <div class="timeline-meta"><%#: Eval("NivelAcademico") %> em <%#: Eval("Curso") %> · desde <%#: Eval("PeriodoFormatado") %></div>
                                                </div>
                                            </div>
                                            <div class="timeline-note" runat="server" visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("CoorientadoresExternos"))) %>'>
                                                Coorientação: <%#: Eval("CoorientadoresExternos") %>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <asp:Panel ID="pnlSemOrientandosAtuais" runat="server" Visible="false">
                                <p class="small muted mb-0">Nenhum orientando ativo no momento.</p>
                            </asp:Panel>
                        </div>
                    </section>

                    <section>
                        <h2 class="section-label">Orientandos egressos</h2>
                        <div class="panel">
                            <div class="timeline">
                                <asp:Repeater ID="rptOrientandosPassados" runat="server">
                                    <ItemTemplate>
                                        <div class="timeline-item">
                                            <div class="d-flex align-items-center gap-2">
                                                <%# AvatarHtml(Eval("EstudanteFotoUrl"), Eval("EstudanteNome"), "avatar avatar-sm") %>
                                                <div>
                                                    <p class="timeline-title"><%#: Eval("EstudanteNome") %></p>
                                                    <div class="timeline-meta"><%#: Eval("NivelAcademico") %> · <%#: Eval("PeriodoFormatado") %> · <%#: Eval("Situacao") %></div>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            <asp:Panel ID="pnlSemOrientandosPassados" runat="server" Visible="false">
                                <p class="small muted mb-0">Nenhum egresso registrado.</p>
                            </asp:Panel>
                        </div>
                    </section>

                    <section>
                        <h2 class="section-label">Todas as publicações</h2>
                        <div class="pub-list">
                            <asp:Repeater ID="rptPublicacoes" runat="server">
                                <ItemTemplate>
                                    <article class="pub-item">
                                        <div class="pub-rank"><%#: Eval("Ano") %></div>
                                        <div>
                                            <h3 class="pub-title">
                                                <a href='<%# UrlAttr(Eval("UrlAcesso")) %>' target="_blank" rel="noopener noreferrer"><%#: Eval("Titulo") %></a>
                                            </h3>
                                            <div class="pub-meta"><span class="vehicle"><%#: Eval("Veiculo") %></span> · <%#: Eval("Tipo") %></div>
                                            <div class="pub-authors"><%#: Eval("Autores") %></div>
                                        </div>
                                        <span class="pub-year"><%#: Eval("Ano") %></span>
                                    </article>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <asp:Panel ID="pnlSemPublicacoes" runat="server" Visible="false" CssClass="empty-state" style="padding:2rem">
                            <p class="mb-0">Nenhuma publicação cadastrada.</p>
                        </asp:Panel>
                    </section>
                </div>

                <aside class="profile-sidebar">
                    <div class="panel">
                        <div class="panel-title">Em números</div>
                        <div class="panel-stat"><span>Projetos vinculados</span><span class="value"><asp:Literal ID="litQtdProjetos" runat="server" /></span></div>
                        <div class="panel-stat"><span>Projetos coordenados</span><span class="value"><asp:Literal ID="litQtdCoordenados" runat="server" /></span></div>
                        <div class="panel-stat"><span>Orientandos atuais</span><span class="value"><asp:Literal ID="litQtdOrientandos" runat="server" /></span></div>
                        <div class="panel-stat"><span>Egressos orientados</span><span class="value"><asp:Literal ID="litQtdEgressos" runat="server" /></span></div>
                        <div class="panel-stat"><span>Publicações</span><span class="value"><asp:Literal ID="litQtdPublicacoes" runat="server" /></span></div>
                        <div class="panel-stat"><span>Verba captada*</span><span class="value" style="font-size:1.05rem"><asp:Literal ID="litVerba" runat="server" /></span></div>
                        <p class="tiny muted mt-2 mb-0">*Soma dos projetos em que participa ou coordena. Valores compartilhados não devem ser somados entre pesquisadores.</p>
                    </div>

                    <div class="panel">
                        <div class="panel-title">Coordenação</div>
                        <asp:Repeater ID="rptProjetosCoordenados" runat="server">
                            <ItemTemplate>
                                <div class="data-row" style="padding:.6rem 0">
                                    <div class="data-row-main">
                                        <div>
                                            <p class="data-row-title" style="font-size:.8rem"><a href='<%# ResolveUrl("~/Projeto.aspx") + "?id=" + Eval("ProjetoId") %>'><%#: Eval("Titulo") %></a></p>
                                            <div class="data-row-sub"><%#: Eval("Situacao") %></div>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlSemCoordenacao" runat="server" Visible="false">
                            <p class="small muted mb-0">Não coordena projetos no momento.</p>
                        </asp:Panel>
                    </div>

                    <div class="panel">
                        <div class="panel-title">Contato institucional</div>
                        <p class="small mb-2"><asp:Literal ID="litInstituicao" runat="server" /></p>
                        <asp:HyperLink ID="lnkEmailContato" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm w-100">Enviar e-mail</asp:HyperLink>
                    </div>
                </aside>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
