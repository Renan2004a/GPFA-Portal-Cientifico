<%@ Page Title="Projetos de pesquisa" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projetos.aspx.cs" Inherits="GPFA.Portal.Projetos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-hero">
        <div class="container-gpfa">
            <div class="eyebrow">Pesquisa · Portfólio</div>
            <h1>Projetos que atravessam fronteiras</h1>
            <p>Iniciativas em andamento, concluídas e em planejamento. Cada projeto reúne coordenação única, pesquisadores colaboradores, estudantes e financiamento transparente.</p>
            <div class="breadcrumb"><a href="<%= ResolveUrl("~/Default.aspx") %>">Início</a> / Projetos</div>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="filter-bar" role="search">
                <div class="filter-field" style="flex:2">
                    <label for="<%= txtBusca.ClientID %>">Buscar projeto</label>
                    <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Título, sigla, coordenador ou tema" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlSituacao.ClientID %>">Situação</label>
                    <asp:DropDownList ID="ddlSituacao" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlLinha.ClientID %>">Linha de pesquisa</label>
                    <asp:DropDownList ID="ddlLinha" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-actions">
                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-gpfa btn-brand js-submit-once" OnClick="btnFiltrar_Click" />
                    <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Projetos.aspx") %>">Limpar</a>
                </div>
            </div>

            <div class="result-count"><asp:Literal ID="litResultado" runat="server" /></div>

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
                                <h2 class="project-title">
                                    <a href='<%# ResolveUrl("~/Projeto.aspx") + "?id=" + Eval("ProjetoId") %>'><%#: Eval("Titulo") %></a>
                                </h2>
                                <p class="project-summary"><%#: Eval("Resumo") %></p>
                                <div class="project-meta">
                                    <span class="project-coord">
                                        <%# AvatarHtml(Eval("CoordenadorFotoUrl"), Eval("CoordenadorNome"), "avatar avatar-sm") %>
                                        <%#: Eval("CoordenadorNome") %>
                                    </span>
                                    <span class="meta-item"><%#: Eval("PeriodoFormatado") %></span>
                                </div>
                                <div class="d-flex justify-content-between align-items-center mt-3 pt-3" style="border-top:1px dashed var(--line)">
                                    <span class="tiny muted"><%#: Eval("QtdPesquisadores") %> pesquisadores · <%#: Eval("QtdEstudantes") %> estudantes</span>
                                    <span class="tiny" style="font-weight:600;color:var(--brand-700)"><%#: Eval("TotalFormatado") %></span>
                                </div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlVazio" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><svg class="icon icon-lg" aria-hidden="true"><use href="#i-flask"/></svg></div>
                <h3>Nenhum projeto encontrado</h3>
                <p>Ajuste os filtros para encontrar outros projetos do grupo.</p>
                <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Projetos.aspx") %>">Limpar filtros</a>
            </asp:Panel>
        </div>
    </section>
</asp:Content>
