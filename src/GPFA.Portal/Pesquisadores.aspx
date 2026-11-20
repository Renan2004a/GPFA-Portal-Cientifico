<%@ Page Title="Pesquisadores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pesquisadores.aspx.cs" Inherits="GPFA.Portal.Pesquisadores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-hero">
        <div class="container-gpfa">
            <div class="eyebrow">Pessoas · Diretório</div>
            <h1>Quem investiga o invisível</h1>
            <p>Pesquisadores do GPFA atuam em física, nanotecnologia, computação, química e engenharia ambiental. Conheça trajetórias, linhas de pesquisa e projetos.</p>
            <div class="breadcrumb"><a href="<%= ResolveUrl("~/Default.aspx") %>">Início</a> / Pesquisadores</div>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="filter-bar" role="search">
                <div class="filter-field" style="flex:2">
                    <label for="<%= txtBusca.ClientID %>">Buscar por nome, formação ou área</label>
                    <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Ex.: nanofotônica, Helena, computação" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlArea.ClientID %>">Área de atuação</label>
                    <asp:DropDownList ID="ddlArea" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-actions">
                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-gpfa btn-brand js-submit-once" OnClick="btnFiltrar_Click" />
                    <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Pesquisadores.aspx") %>">Limpar</a>
                </div>
            </div>

            <div class="result-count"><asp:Literal ID="litResultado" runat="server" /></div>

            <div class="card-grid grid-3">
                <asp:Repeater ID="rptPesquisadores" runat="server">
                    <ItemTemplate>
                        <article class="person-card">
                            <div class="person-head">
                                <%# AvatarHtml(Eval("FotoUrl"), Eval("NomeCompleto")) %>
                                <div>
                                    <h2 class="person-name">
                                        <a href='<%# ResolveUrl("~/Pesquisador.aspx") + "?id=" + Eval("PesquisadorId") %>'><%#: Eval("NomeCompleto") %></a>
                                    </h2>
                                    <div class="person-role"><%#: Eval("TitulacaoExibicao") %><br /><%#: Eval("Departamento") %></div>
                                </div>
                            </div>
                            <span class="person-area"><%#: Eval("AreaAtuacao") %></span>
                            <p class="person-bio"><%#: Eval("MiniBiografia") %></p>
                            <div class="d-flex justify-content-between align-items-center gap-2 mb-3">
                                <div class="external-links">
                                    <a runat="server" visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("GoogleScholarUrl"))) %>'
                                       href='<%# UrlAttr(Eval("GoogleScholarUrl")) %>' target="_blank" rel="noopener noreferrer" title="Google Scholar" aria-label="Google Scholar">
                                        <span class="tiny" style="font-weight:700">GS</span>
                                    </a>
                                    <a runat="server" visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("OrcidUrl"))) %>'
                                       href='<%# UrlAttr(Eval("OrcidUrl")) %>' target="_blank" rel="noopener noreferrer" title="ORCID" aria-label="ORCID">
                                        <span class="tiny" style="font-weight:700">iD</span>
                                    </a>
                                    <a runat="server" visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("LattesUrl"))) %>'
                                       href='<%# UrlAttr(Eval("LattesUrl")) %>' target="_blank" rel="noopener noreferrer" title="Currículo Lattes" aria-label="Currículo Lattes">
                                        <span class="tiny" style="font-weight:700">CV</span>
                                    </a>
                                </div>
                                <span class="tiny muted"><%#: Eval("FormacaoAcademica") %></span>
                            </div>
                            <div class="person-foot">
                                <div class="person-stats">
                                    <span><strong><%#: Eval("QtdProjetos") %></strong> projetos</span>
                                    <span><strong><%#: Eval("QtdOrientandosAtuais") %></strong> orientandos</span>
                                    <span><strong><%#: Eval("QtdPublicacoes") %></strong> artigos</span>
                                </div>
                                <a class="btn-link-gpfa" href='<%# ResolveUrl("~/Pesquisador.aspx") + "?id=" + Eval("PesquisadorId") %>'>Perfil →</a>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlVazio" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><svg class="icon icon-lg" aria-hidden="true"><use href="#i-users"/></svg></div>
                <h3>Nenhum pesquisador encontrado</h3>
                <p>Tente ajustar a busca ou selecionar outra área de atuação.</p>
                <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Pesquisadores.aspx") %>">Limpar filtros</a>
            </asp:Panel>
        </div>
    </section>
</asp:Content>
