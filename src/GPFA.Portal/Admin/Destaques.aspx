<%@ Page Title="Destaques" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Destaques.aspx.cs" Inherits="GPFA.Portal.Admin.DestaquesAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Publicações em destaque</h2>
            <p>Cada pesquisador mantém no máximo cinco destaques, sempre entre suas próprias publicações (RN05).</p>
        </div>
    </div>

    <div class="filter-bar">
        <div class="filter-field" style="flex:3">
            <label for="<%= ddlPesquisador.ClientID %>">Pesquisador</label>
            <asp:DropDownList ID="ddlPesquisador" runat="server" CssClass="form-select" AutoPostBack="true"
                OnSelectedIndexChanged="ddlPesquisador_SelectedIndexChanged" />
        </div>
        <div class="filter-actions">
            <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Admin/Publicacoes.aspx") %>">Gerenciar publicações</a>
        </div>
    </div>

    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <div class="admin-grid">
            <div class="panel-card">
                <div class="panel-card-head">
                    <h3>Destaques selecionados</h3>
                    <span class="tiny muted"><asp:Literal ID="litContagem" runat="server" /> de 5</span>
                </div>
                <div class="table-responsive">
                    <asp:GridView ID="gvDestaques" runat="server" AutoGenerateColumns="false" DataKeyNames="DestaqueId"
                        CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                        OnRowCommand="gvDestaques_RowCommand" EmptyDataText="Nenhuma publicação em destaque.">
                        <Columns>
                            <asp:TemplateField HeaderText="Posição">
                                <ItemTemplate><span class="pub-year"><%#: Eval("Ordem") %>º</span></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Publicação">
                                <ItemTemplate>
                                    <strong><%#: Eval("Titulo") %></strong><br />
                                    <span class="tiny muted"><%#: Eval("Veiculo") %> · <%#: Eval("Ano") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkRemover" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                        CommandName="Remover" CommandArgument='<%# Eval("DestaqueId") %>' CausesValidation="false"
                                        OnClientClick="return confirm('Remover este destaque? A publicação continua no perfil.');">Remover</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataRowStyle CssClass="text-muted" />
                    </asp:GridView>
                </div>
            </div>

            <div class="panel-card">
                <div class="panel-card-head"><h3>Adicionar destaque</h3></div>
                <label class="form-label" for="<%= ddlPublicacao.ClientID %>">Publicação do pesquisador</label>
                <asp:DropDownList ID="ddlPublicacao" runat="server" CssClass="form-select mb-2" />
                <label class="form-label" for="<%= ddlOrdem.ClientID %>">Posição (1 a 5)</label>
                <asp:DropDownList ID="ddlOrdem" runat="server" CssClass="form-select mb-2" />
                <asp:Button ID="btnAdicionar" runat="server" Text="Adicionar destaque" CssClass="btn-gpfa btn-brand btn-sm w-100 js-submit-once"
                    OnClick="btnAdicionar_Click" CausesValidation="false" />
                <p class="tiny muted mt-3 mb-0">O sistema bloqueia publicações de outra autoria, posições ocupadas e um sexto destaque.</p>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
