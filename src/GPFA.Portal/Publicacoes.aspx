<%@ Page Title="Publicações científicas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Publicacoes.aspx.cs" Inherits="GPFA.Portal.Publicacoes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-hero">
        <div class="container-gpfa">
            <div class="eyebrow">Conhecimento · Produção científica</div>
            <h1>O que aprendemos e compartilhamos</h1>
            <p>Artigos, capítulos e trabalhos em conferências produzidos pelo grupo. Filtre por ano, pesquisador ou projeto e acesse o DOI quando disponível.</p>
            <div class="breadcrumb"><a href="<%= ResolveUrl("~/Default.aspx") %>">Início</a> / Publicações</div>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="filter-bar" role="search">
                <div class="filter-field" style="flex:2">
                    <label for="<%= txtBusca.ClientID %>">Buscar</label>
                    <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Título, veículo ou autor" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlAno.ClientID %>">Ano</label>
                    <asp:DropDownList ID="ddlAno" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlPesquisador.ClientID %>">Pesquisador</label>
                    <asp:DropDownList ID="ddlPesquisador" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlProjeto.ClientID %>">Projeto</label>
                    <asp:DropDownList ID="ddlProjeto" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-actions">
                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-gpfa btn-brand js-submit-once" OnClick="btnFiltrar_Click" />
                    <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Publicacoes.aspx") %>">Limpar</a>
                </div>
            </div>

            <div class="result-count"><asp:Literal ID="litResultado" runat="server" /></div>

            <div class="pub-list">
                <asp:Repeater ID="rptPublicacoes" runat="server">
                    <ItemTemplate>
                        <article class="pub-item">
                            <div class="pub-rank"><%#: Eval("Ano") %></div>
                            <div>
                                <h2 class="pub-title">
                                    <a href='<%# UrlAttr(Eval("UrlAcesso")) %>' target="_blank" rel="noopener noreferrer"><%#: Eval("Titulo") %></a>
                                </h2>
                                <div class="pub-meta">
                                    <span class="vehicle"><%#: Eval("Veiculo") %></span> · <%#: Eval("Tipo") %>
                                    <span runat="server" visible='<%# Eval("ProjetoSigla") != null %>'> · projeto <strong><%#: Eval("ProjetoSigla") %></strong></span>
                                </div>
                                <div class="pub-authors"><%#: Eval("Autores") %></div>
                            </div>
                            <div class="text-end">
                                <span class="pub-year"><%#: Eval("Ano") %></span>
                                <div class="mt-2"><a class="btn-gpfa btn-outline-brand btn-sm" href='<%# UrlAttr(Eval("UrlAcesso")) %>' target="_blank" rel="noopener noreferrer">Acessar</a></div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlVazio" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><svg class="icon icon-lg" aria-hidden="true"><use href="#i-book"/></svg></div>
                <h3>Nenhuma publicação encontrada</h3>
                <p>Altere os filtros ou limpe a busca para ver toda a produção científica.</p>
                <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Publicacoes.aspx") %>">Limpar filtros</a>
            </asp:Panel>

            <div class="d-flex justify-content-between align-items-center mt-3">
                <asp:LinkButton ID="lnkAnterior" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm" OnClick="lnkAnterior_Click" CausesValidation="false">
                    <svg class="icon icon-sm" aria-hidden="true"><use href="#i-arrow-right"/></svg> Anterior
                </asp:LinkButton>
                <span class="small muted"><asp:Literal ID="litPagina" runat="server" /></span>
                <asp:LinkButton ID="lnkProxima" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm" OnClick="lnkProxima_Click" CausesValidation="false">
                    Próxima <svg class="icon icon-sm" aria-hidden="true"><use href="#i-arrow-right"/></svg>
                </asp:LinkButton>
            </div>
        </div>
    </section>
</asp:Content>
