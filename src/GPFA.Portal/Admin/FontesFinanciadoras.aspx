<%@ Page Title="Fontes financiadoras" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="FontesFinanciadoras.aspx.cs" Inherits="GPFA.Portal.Admin.FontesFinanciadorasAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Fontes financiadoras</h2>
            <p>Agências, fundações e parcerias privadas usadas nos registros de financiamento.</p>
        </div>
        <asp:Button ID="btnNova" runat="server" Text="Nova fonte" CssClass="btn-gpfa btn-brand js-submit-once"
            OnClick="btnNova_Click" CausesValidation="false" />
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2><asp:Literal ID="litTituloForm" runat="server" /></h2>
                <p>Fontes em uso não podem ser excluídas (integridade referencial).</p>
            </div>
            <asp:Button ID="btnFecharForm" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>

        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
        <asp:ValidationSummary ID="vsFonte" runat="server" CssClass="validation-summary"
            HeaderText="Corrija os campos destacados:" ValidationGroup="fonte" />

        <div class="form-grid">
            <div class="col-6">
                <label class="form-label" for="<%= txtNome.ClientID %>">Nome <span class="req">*</span></label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="120" ValidationGroup="fonte" />
                <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome" CssClass="field-error"
                    ErrorMessage="Informe o nome." ValidationGroup="fonte" Display="Dynamic" />
            </div>
            <div class="col-2">
                <label class="form-label" for="<%= txtSigla.ClientID %>">Sigla</label>
                <asp:TextBox ID="txtSigla" runat="server" CssClass="form-control" MaxLength="30" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= ddlTipo.ClientID %>">Tipo <span class="req">*</span></label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select" ValidationGroup="fonte" />
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= txtSite.ClientID %>">Site oficial</label>
                <asp:TextBox ID="txtSite" runat="server" CssClass="form-control" MaxLength="300" placeholder="https://..." />
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar fonte" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnSalvar_Click" ValidationGroup="fonte" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn-gpfa btn-outline-brand"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div class="table-shell">
        <div class="table-responsive">
            <asp:GridView ID="gvFontes" runat="server" AutoGenerateColumns="false" DataKeyNames="FonteId"
                CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                OnRowCommand="gvFontes_RowCommand" EmptyDataText="Nenhuma fonte cadastrada.">
                <Columns>
                    <asp:TemplateField HeaderText="Fonte">
                        <ItemTemplate><strong><%#: Eval("NomeExibicao") %></strong><br /><span class="tiny muted"><%#: Eval("Nome") %></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Tipo" HeaderText="Tipo" />
                    <asp:BoundField DataField="QtdProjetos" HeaderText="Projetos" ItemStyle-CssClass="text-center" />
                    <asp:BoundField DataField="QtdFinanciamentos" HeaderText="Registros" ItemStyle-CssClass="text-center" />
                    <asp:BoundField DataField="TotalCaptado" HeaderText="Total" DataFormatString="{0:C0}" ItemStyle-CssClass="nowrap" />
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEditar" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                CommandName="Editar" CommandArgument='<%# Eval("FonteId") %>' CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                CommandName="Excluir" CommandArgument='<%# Eval("FonteId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Excluir esta fonte? A operação é bloqueada se houver financiamentos.');">Excluir</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataRowStyle CssClass="text-muted" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
