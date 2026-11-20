<%@ Page Title="Estudantes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Estudantes.aspx.cs" Inherits="GPFA.Portal.Estudantes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="page-hero">
        <div class="container-gpfa">
            <div class="eyebrow">Pessoas · Formação</div>
            <h1>Estudantes em formação</h1>
            <p>Da iniciação científica ao doutorado, cada estudante do GPFA está vinculado a pelo menos um projeto de pesquisa e a um orientador principal do grupo.</p>
            <div class="breadcrumb"><a href="<%= ResolveUrl("~/Default.aspx") %>">Início</a> / Estudantes</div>
        </div>
    </section>

    <section class="section">
        <div class="container-gpfa">
            <div class="filter-bar" role="search">
                <div class="filter-field" style="flex:2">
                    <label for="<%= txtBusca.ClientID %>">Buscar</label>
                    <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Nome, curso, orientador ou projeto" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlNivel.ClientID %>">Nível</label>
                    <asp:DropDownList ID="ddlNivel" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlSituacao.ClientID %>">Situação</label>
                    <asp:DropDownList ID="ddlSituacao" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-field">
                    <label for="<%= ddlOrientador.ClientID %>">Orientador</label>
                    <asp:DropDownList ID="ddlOrientador" runat="server" CssClass="form-select" />
                </div>
                <div class="filter-actions">
                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-gpfa btn-brand js-submit-once" OnClick="btnFiltrar_Click" />
                    <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Estudantes.aspx") %>">Limpar</a>
                </div>
            </div>

            <div class="result-count"><asp:Literal ID="litResultado" runat="server" /></div>

            <div class="card-grid grid-2">
                <asp:Repeater ID="rptEstudantes" runat="server">
                    <ItemTemplate>
                        <article class="person-card">
                            <div class="person-head">
                                <%# AvatarHtml(Eval("FotoUrl"), Eval("NomeCompleto")) %>
                                <div style="min-width:0">
                                    <h2 class="person-name"><%#: Eval("NomeCompleto") %></h2>
                                    <div class="person-role"><%#: Eval("DescricaoVinculo") %></div>
                                </div>
                                <span class='status-pill <%# Eval("SituacaoCss") %> ms-auto'><%#: Eval("Situacao") %></span>
                            </div>

                            <div class="row g-2 small mb-3">
                                <div class="col-sm-6">
                                    <div class="tiny muted text-uppercase" style="letter-spacing:.08em">Orientador principal</div>
                                    <div><%#: Eval("OrientadorNome") %></div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="tiny muted text-uppercase" style="letter-spacing:.08em">Ingresso</div>
                                    <div><%#: Eval("IngresoFormatado") %></div>
                                </div>
                                <div class="col-12" runat="server" visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("CoorientadoresExternos"))) %>'>
                                    <div class="tiny muted text-uppercase" style="letter-spacing:.08em">Coorientadores</div>
                                    <div><%#: Eval("CoorientadoresExternos") %></div>
                                </div>
                            </div>

                            <div class="mt-auto">
                                <div class="tiny muted text-uppercase mb-1" style="letter-spacing:.08em">Projetos vinculados</div>
                                <div class="tag-list">
                                    <asp:Repeater ID="rptProjetosEstudante" runat="server" DataSource='<%# Eval("Projetos") %>'>
                                        <ItemTemplate>
                                            <a class="tag" style="text-decoration:none" href='<%# ResolveUrl("~/Projeto.aspx") + "?id=" + Eval("ProjetoId") %>'><%#: Eval("Sigla") ?? Eval("Titulo") %></a>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <div class="mt-3 tiny muted"><%#: Eval("FormacaoAcademica") %></div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <asp:Panel ID="pnlVazio" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon"><svg class="icon icon-lg" aria-hidden="true"><use href="#i-graduation"/></svg></div>
                <h3>Nenhum estudante encontrado</h3>
                <p>Ajuste os filtros para ver outros perfis.</p>
                <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Estudantes.aspx") %>">Limpar filtros</a>
            </asp:Panel>
        </div>
    </section>
</asp:Content>
