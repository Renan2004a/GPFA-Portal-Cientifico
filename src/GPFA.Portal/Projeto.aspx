<%@ Page Title="Projeto de pesquisa" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projeto.aspx.cs" Inherits="GPFA.Portal.Projeto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="pnlNaoEncontrado" runat="server" Visible="false">
        <section class="page-hero">
            <div class="container-gpfa">
                <div class="eyebrow">Pesquisa · Projeto</div>
                <h1>Projeto não encontrado</h1>
                <p>O projeto solicitado não existe ou foi removido. Consulte o portfólio atualizado.</p>
                <div class="mt-3"><a class="btn-gpfa btn-accent" href="<%= ResolveUrl("~/Projetos.aspx") %>">Ver projetos</a></div>
            </div>
        </section>
    </asp:Panel>

    <asp:Panel ID="pnlProjeto" runat="server" Visible="false">
        <section class="page-hero">
            <div class="container-gpfa">
                <div class="eyebrow"><asp:Literal ID="litLinha" runat="server" /></div>
                <div class="d-flex align-items-center gap-3 flex-wrap mb-2">
                    <span id="spnStatus" runat="server" class="status-pill"></span>
                    <span class="tiny" style="color:#9fb5c2;letter-spacing:.08em;text-transform:uppercase"><asp:Literal ID="litSigla" runat="server" /></span>
                </div>
                <h1><asp:Literal ID="litTitulo" runat="server" /></h1>
                <p><asp:Literal ID="litResumo" runat="server" /></p>
                <div class="breadcrumb"><a href="<%= ResolveUrl("~/Default.aspx") %>">Início</a> / <a href="<%= ResolveUrl("~/Projetos.aspx") %>">Projetos</a> / <asp:Literal ID="litSiglaCrumb" runat="server" /></div>
            </div>
        </section>

        <div class="container-gpfa">
            <div class="mt-4 detail-hero-media">
                <asp:Image ID="imgProjeto" runat="server" AlternateText="Imagem ilustrativa do projeto" />
            </div>

            <div class="detail-grid">
                <div>
                    <section class="mb-4">
                        <h2 class="section-label">Sobre o projeto</h2>
                        <div class="bio-text"><asp:Literal ID="litDescricao" runat="server" /></div>
                    </section>

                    <section class="mb-4">
                        <h2 class="section-label">Equipe de pesquisa</h2>
                        <div class="panel mb-3" style="padding:1.1rem">
                            <div class="d-flex align-items-center gap-3">
                                <asp:Literal ID="litCoordenadorAvatar" runat="server" />
                                <div>
                                    <div class="tiny muted text-uppercase" style="letter-spacing:.08em">Coordenação</div>
                                    <a class="person-name" style="font-size:1rem" href='<%= ResolveUrl("~/Pesquisador.aspx") + "?id=" + hdnCoordenadorId.Value %>'><asp:Literal ID="litCoordenadorNome" runat="server" /></a>
                                    <div class="tiny muted"><asp:Literal ID="litCoordenadorFormacao" runat="server" /></div>
                                </div>
                                <span class="status-pill status-concluido ms-auto">Coordenador(a)</span>
                            </div>
                        </div>
                        <asp:HiddenField ID="hdnCoordenadorId" runat="server" />

                        <div class="data-list">
                            <asp:Repeater ID="rptParticipantes" runat="server">
                                <ItemTemplate>
                                    <div class="data-row">
                                        <div class="data-row-main">
                                            <%# AvatarHtml(Eval("PesquisadorFotoUrl"), Eval("PesquisadorNome"), "avatar avatar-sm") %>
                                            <div>
                                                <p class="data-row-title"><%#: Eval("PesquisadorNome") %></p>
                                                <div class="data-row-sub"><%#: Eval("Papel") %> · desde <%#: Eval("EntradaFormatada") %></div>
                                            </div>
                                        </div>
                                        <a class="btn-link-gpfa" href='<%# ResolveUrl("~/Pesquisador.aspx") + "?id=" + Eval("PesquisadorId") %>'>Perfil →</a>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <asp:Panel ID="pnlSemParticipantes" runat="server" Visible="false" CssClass="small muted mt-2">
                            Além da coordenação, este projeto não possui outros pesquisadores cadastrados no momento.
                        </asp:Panel>
                    </section>

                    <section class="mb-4">
                        <h2 class="section-label">Estudantes vinculados</h2>
                        <div class="data-list">
                            <asp:Repeater ID="rptEstudantes" runat="server">
                                <ItemTemplate>
                                    <div class="data-row">
                                        <div class="data-row-main">
                                            <%# AvatarHtml(Eval("FotoUrl"), Eval("NomeCompleto"), "avatar avatar-sm") %>
                                            <div>
                                                <p class="data-row-title"><%#: Eval("NomeCompleto") %></p>
                                                <div class="data-row-sub"><%#: Eval("DescricaoVinculo") %> · orientação: <%#: Eval("OrientadorNome") %></div>
                                            </div>
                                        </div>
                                        <span class='status-pill <%# Eval("SituacaoCss") %>'><%#: Eval("Situacao") %></span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <asp:Panel ID="pnlSemEstudantes" runat="server" Visible="false" CssClass="small muted mt-2">
                            Nenhum estudante vinculado atualmente.
                        </asp:Panel>
                    </section>

                    <section class="mb-4">
                        <h2 class="section-label">Publicações associadas</h2>
                        <div class="pub-list">
                            <asp:Repeater ID="rptPublicacoes" runat="server">
                                <ItemTemplate>
                                    <article class="pub-item">
                                        <div class="pub-rank"><%#: Eval("Ano") %></div>
                                        <div>
                                            <h3 class="pub-title">
                                                <a href='<%# UrlAttr(Eval("UrlAcesso")) %>' target="_blank" rel="noopener noreferrer"><%#: Eval("Titulo") %></a>
                                            </h3>
                                            <div class="pub-meta"><span class="vehicle"><%#: Eval("Veiculo") %></span> · <%#: Eval("Autores") %></div>
                                        </div>
                                        <span class="pub-year"><%#: Eval("Ano") %></span>
                                    </article>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <asp:Panel ID="pnlSemPublicacoes" runat="server" Visible="false" CssClass="small muted mt-2">
                            Nenhuma publicação vinculada a este projeto.
                        </asp:Panel>
                    </section>

                    <section>
                        <h2 class="section-label">Financiamentos</h2>
                        <div class="table-shell">
                            <div class="table-responsive">
                                <table class="table-gpfa">
                                    <thead>
                                        <tr>
                                            <th>Fonte</th>
                                            <th>Descrição / processo</th>
                                            <th>Período</th>
                                            <th style="text-align:right">Valor</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptFinanciamentos" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><strong><%#: Eval("FonteSigla") %></strong><br /><span class="tiny muted"><%#: Eval("FonteNome") %></span></td>
                                                    <td><%#: Eval("Descricao") %><br /><span class="tiny muted"><%#: Eval("NumeroProcesso") %></span></td>
                                                    <td class="nowrap"><%#: Eval("PeriodoFormatado") %></td>
                                                    <td style="text-align:right;font-weight:600"><%#: Eval("ValorFormatado") %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <asp:Panel ID="pnlSemFinanciamentos" runat="server" Visible="false" CssClass="small muted mt-2">
                            Nenhum financiamento registrado para este projeto.
                        </asp:Panel>
                    </section>
                </div>

                <aside class="profile-sidebar">
                    <div class="panel">
                        <div class="panel-title">Ficha do projeto</div>
                        <div class="panel-stat"><span>Situação</span><span class="value" style="font-size:.95rem"><asp:Literal ID="litSituacao" runat="server" /></span></div>
                        <div class="panel-stat"><span>Início</span><span class="value" style="font-size:.95rem"><asp:Literal ID="litInicio" runat="server" /></span></div>
                        <div class="panel-stat"><span>Término</span><span class="value" style="font-size:.95rem"><asp:Literal ID="litFim" runat="server" /></span></div>
                        <div class="panel-stat"><span>Pesquisadores</span><span class="value"><asp:Literal ID="litQtdPesquisadores" runat="server" /></span></div>
                        <div class="panel-stat"><span>Estudantes</span><span class="value"><asp:Literal ID="litQtdEstudantes" runat="server" /></span></div>
                        <div class="panel-stat"><span>Publicações</span><span class="value"><asp:Literal ID="litQtdPublicacoes" runat="server" /></span></div>
                        <div class="panel-stat"><span>Verba captada</span><span class="value" style="font-size:1.15rem;color:var(--success)"><asp:Literal ID="litTotal" runat="server" /></span></div>
                        <p class="tiny muted mt-2 mb-0">Somatório dos financiamentos registrados para este projeto, sem duplicidade.</p>
                    </div>

                    <div class="panel">
                        <div class="panel-title">Linha de pesquisa</div>
                        <span class="tag"><asp:Literal ID="litLinhaSidebar" runat="server" /></span>
                    </div>

                    <div class="panel">
                        <div class="panel-title">Fontes financiadoras</div>
                        <div class="data-list">
                            <asp:Repeater ID="rptFontes" runat="server">
                                <ItemTemplate>
                                    <div class="data-row" style="padding:.55rem 0">
                                        <span class="small"><%#: Eval("FonteSigla") %></span>
                                        <span class="data-row-value" style="font-size:.78rem"><%#: Eval("ValorFormatado") %></span>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </aside>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
