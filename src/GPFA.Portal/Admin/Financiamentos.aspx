<%@ Page Title="Financiamentos" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Financiamentos.aspx.cs" Inherits="GPFA.Portal.Admin.FinanciamentosAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Financiamentos</h2>
            <p>Verbas captadas por projeto e fonte. Valores positivos e somados uma única vez por projeto (RN06). <asp:Literal ID="litTotal" runat="server" /></p>
        </div>
        <asp:Button ID="btnNovo" runat="server" Text="Novo financiamento" CssClass="btn-gpfa btn-brand js-submit-once"
            OnClick="btnNovo_Click" CausesValidation="false" />
    </div>

    <div class="filter-bar">
        <div class="filter-field" style="flex:2">
            <label for="<%= txtBusca.ClientID %>">Buscar</label>
            <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Projeto, fonte, descrição ou processo" />
        </div>
        <div class="filter-field">
            <label for="<%= ddlFiltroProjeto.ClientID %>">Projeto</label>
            <asp:DropDownList ID="ddlFiltroProjeto" runat="server" CssClass="form-select" />
        </div>
        <div class="filter-field">
            <label for="<%= ddlFiltroFonte.ClientID %>">Fonte</label>
            <asp:DropDownList ID="ddlFiltroFonte" runat="server" CssClass="form-select" />
        </div>
        <div class="filter-actions">
            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnFiltrar_Click" CausesValidation="false" />
            <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Admin/Financiamentos.aspx") %>">Limpar</a>
        </div>
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2><asp:Literal ID="litTituloForm" runat="server" /></h2>
                <p>O valor deve ser maior que zero. A moeda de contabilização do portal é o real (BRL).</p>
            </div>
            <asp:Button ID="btnFecharForm" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>

        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
        <asp:ValidationSummary ID="vsFinanciamento" runat="server" CssClass="validation-summary"
            HeaderText="Corrija os campos destacados:" ValidationGroup="fin" />

        <div class="form-grid">
            <div class="col-7">
                <label class="form-label" for="<%= ddlProjeto.ClientID %>">Projeto <span class="req">*</span></label>
                <asp:DropDownList ID="ddlProjeto" runat="server" CssClass="form-select" ValidationGroup="fin" />
                <asp:RequiredFieldValidator ID="rfvProjeto" runat="server" ControlToValidate="ddlProjeto" CssClass="field-error"
                    ErrorMessage="Selecione o projeto." InitialValue="" ValidationGroup="fin" Display="Dynamic" />
            </div>
            <div class="col-5">
                <label class="form-label" for="<%= ddlFonte.ClientID %>">Fonte financiadora <span class="req">*</span></label>
                <asp:DropDownList ID="ddlFonte" runat="server" CssClass="form-select" ValidationGroup="fin" />
                <asp:RequiredFieldValidator ID="rfvFonte" runat="server" ControlToValidate="ddlFonte" CssClass="field-error"
                    ErrorMessage="Selecione a fonte." InitialValue="" ValidationGroup="fin" Display="Dynamic" />
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= txtDescricao.ClientID %>">Descrição <span class="req">*</span></label>
                <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" MaxLength="200" ValidationGroup="fin" />
                <asp:RequiredFieldValidator ID="rfvDescricao" runat="server" ControlToValidate="txtDescricao" CssClass="field-error"
                    ErrorMessage="Informe a descrição." ValidationGroup="fin" Display="Dynamic" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtProcesso.ClientID %>">Número do processo</label>
                <asp:TextBox ID="txtProcesso" runat="server" CssClass="form-control" MaxLength="80" />
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= txtValor.ClientID %>">Valor (R$) <span class="req">*</span></label>
                <asp:TextBox ID="txtValor" runat="server" CssClass="form-control" placeholder="120000,00" ValidationGroup="fin" />
                <asp:RequiredFieldValidator ID="rfvValor" runat="server" ControlToValidate="txtValor" CssClass="field-error"
                    ErrorMessage="Informe o valor." ValidationGroup="fin" Display="Dynamic" />
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= txtInicio.ClientID %>">Início <span class="req">*</span></label>
                <asp:TextBox ID="txtInicio" runat="server" CssClass="form-control" TextMode="Date" ValidationGroup="fin" />
                <asp:RequiredFieldValidator ID="rfvInicio" runat="server" ControlToValidate="txtInicio" CssClass="field-error"
                    ErrorMessage="Informe o início." ValidationGroup="fin" Display="Dynamic" />
            </div>
            <div class="col-2">
                <label class="form-label" for="<%= txtFim.ClientID %>">Fim</label>
                <asp:TextBox ID="txtFim" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar financiamento" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnSalvar_Click" ValidationGroup="fin" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn-gpfa btn-outline-brand"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div class="table-shell">
        <div class="table-responsive">
            <asp:GridView ID="gvFinanciamentos" runat="server" AutoGenerateColumns="false" DataKeyNames="FinanciamentoId"
                CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                AllowPaging="true" PageSize="12" OnPageIndexChanging="gvFinanciamentos_PageIndexChanging"
                OnRowCommand="gvFinanciamentos_RowCommand" EmptyDataText="Nenhum financiamento cadastrado.">
                <Columns>
                    <asp:TemplateField HeaderText="Projeto">
                        <ItemTemplate>
                            <strong><%#: Eval("ProjetoTitulo") %></strong><br />
                            <span class="tiny muted"><%#: Eval("ProjetoSigla") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Fonte">
                        <ItemTemplate><strong><%#: Eval("FonteSigla") %></strong><br /><span class="tiny muted"><%#: Eval("FonteNome") %></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Descrição">
                        <ItemTemplate><%#: Eval("Descricao") %><br /><span class="tiny muted"><%#: Eval("NumeroProcesso") %></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="DataInicio" HeaderText="Início" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Valor" HeaderText="Valor" DataFormatString="{0:C2}" ItemStyle-CssClass="nowrap" />
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEditar" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                CommandName="Editar" CommandArgument='<%# Eval("FinanciamentoId") %>' CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                CommandName="Excluir" CommandArgument='<%# Eval("FinanciamentoId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Excluir este registro de financiamento?');">Excluir</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="grid-pager" />
                <EmptyDataRowStyle CssClass="text-muted" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
