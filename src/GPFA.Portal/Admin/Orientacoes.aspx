<%@ Page Title="Orientações" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Orientacoes.aspx.cs" Inherits="GPFA.Portal.Admin.OrientacoesAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Orientações</h2>
            <p>Histórico de orientações principais e coorientações internas (RN02 e RN04). <asp:Literal ID="litTotal" runat="server" /></p>
        </div>
        <asp:Button ID="btnNova" runat="server" Text="Nova orientação" CssClass="btn-gpfa btn-brand js-submit-once"
            OnClick="btnNova_Click" CausesValidation="false" />
    </div>

    <div class="filter-bar">
        <div class="filter-field" style="flex:2">
            <label for="<%= txtBusca.ClientID %>">Buscar</label>
            <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Estudante ou orientador" />
        </div>
        <div class="filter-field">
            <label for="<%= ddlVinculo.ClientID %>">Vínculo</label>
            <asp:DropDownList ID="ddlVinculo" runat="server" CssClass="form-select">
                <asp:ListItem Text="Todos" Value="" />
                <asp:ListItem Text="Atuais" Value="Atual" />
                <asp:ListItem Text="Passados" Value="Passado" />
            </asp:DropDownList>
        </div>
        <div class="filter-actions">
            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnFiltrar_Click" CausesValidation="false" />
        </div>
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2>Nova orientação</h2>
                <p>Ao definir um novo orientador principal, a orientação principal anterior é encerrada automaticamente na mesma data.</p>
            </div>
            <asp:Button ID="btnFecharForm" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnFecharForm_Click" CausesValidation="false" />
        </div>
        <asp:ValidationSummary ID="vsOrientacao" runat="server" CssClass="validation-summary"
            HeaderText="Corrija os campos:" ValidationGroup="ori" />
        <div class="form-grid">
            <div class="col-6">
                <label class="form-label" for="<%= ddlEstudante.ClientID %>">Estudante <span class="req">*</span></label>
                <asp:DropDownList ID="ddlEstudante" runat="server" CssClass="form-select" ValidationGroup="ori" />
                <asp:RequiredFieldValidator ID="rfvEstudante" runat="server" ControlToValidate="ddlEstudante" CssClass="field-error"
                    ErrorMessage="Selecione o estudante." InitialValue="" ValidationGroup="ori" Display="Dynamic" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= ddlOrientador.ClientID %>">Orientador (pesquisador cadastrado) <span class="req">*</span></label>
                <asp:DropDownList ID="ddlOrientador" runat="server" CssClass="form-select" ValidationGroup="ori" />
                <asp:RequiredFieldValidator ID="rfvOrientador" runat="server" ControlToValidate="ddlOrientador" CssClass="field-error"
                    ErrorMessage="Selecione o orientador." InitialValue="" ValidationGroup="ori" Display="Dynamic" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= ddlTipo.ClientID %>">Tipo <span class="req">*</span></label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select" ValidationGroup="ori">
                    <asp:ListItem Text="Orientador principal" Value="Principal" />
                    <asp:ListItem Text="Coorientação interna" Value="Coorientacao" />
                </asp:DropDownList>
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtInicio.ClientID %>">Início <span class="req">*</span></label>
                <asp:TextBox ID="txtInicio" runat="server" CssClass="form-control" TextMode="Date" ValidationGroup="ori" />
                <asp:RequiredFieldValidator ID="rfvInicio" runat="server" ControlToValidate="txtInicio" CssClass="field-error"
                    ErrorMessage="Informe o início." ValidationGroup="ori" Display="Dynamic" />
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= txtObservacao.ClientID %>">Observação</label>
                <asp:TextBox ID="txtObservacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="300" />
            </div>
        </div>
        <div class="form-actions">
            <asp:Button ID="btnSalvar" runat="server" Text="Registrar orientação" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnSalvar_Click" ValidationGroup="ori" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlEncerrar" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2>Encerrar orientação</h2>
                <p><asp:Literal ID="litEncerrarInfo" runat="server" /></p>
            </div>
            <asp:Button ID="btnFecharEncerrar" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnFecharEncerrar_Click" CausesValidation="false" />
        </div>
        <asp:HiddenField ID="hdnOrientacaoId" runat="server" Value="0" />
        <div class="form-grid">
            <div class="col-4">
                <label class="form-label" for="<%= txtFim.ClientID %>">Data de encerramento</label>
                <asp:TextBox ID="txtFim" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-8 d-flex align-items-end">
                <asp:Button ID="btnEncerrar" runat="server" Text="Confirmar encerramento" CssClass="btn-gpfa btn-brand js-submit-once"
                    OnClick="btnEncerrar_Click" CausesValidation="false" />
            </div>
        </div>
        <p class="tiny muted mt-2 mb-0">Se este for o único orientador principal ativo de um estudante ativo, o sistema bloqueará a operação até que outro orientador seja definido.</p>
    </asp:Panel>

    <div class="table-shell">
        <div class="table-responsive">
            <asp:GridView ID="gvOrientacoes" runat="server" AutoGenerateColumns="false" DataKeyNames="OrientacaoId"
                CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                AllowPaging="true" PageSize="12" OnPageIndexChanging="gvOrientacoes_PageIndexChanging"
                OnRowCommand="gvOrientacoes_RowCommand" EmptyDataText="Nenhuma orientação registrada.">
                <Columns>
                    <asp:BoundField DataField="EstudanteNome" HeaderText="Estudante" />
                    <asp:BoundField DataField="EstudanteNivel" HeaderText="Nível" />
                    <asp:BoundField DataField="OrientadorNome" HeaderText="Orientador" />
                    <asp:TemplateField HeaderText="Tipo">
                        <ItemTemplate>
                            <%# Convert.ToBoolean(Eval("Principal")) ? "Principal" : "Coorientação interna" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="DataInicio" HeaderText="Início" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="DataFim" HeaderText="Fim" DataFormatString="{0:dd/MM/yyyy}" NullDisplayText="—" />
                    <asp:TemplateField HeaderText="Vínculo">
                        <ItemTemplate>
                            <span class='status-pill <%# Eval("Vinculo").ToString() == "Atual" ? "status-andamento" : "status-neutro" %>'><%#: Eval("Vinculo") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEncerrar" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                CommandName="Encerrar" CommandArgument='<%# Eval("OrientacaoId") %>' CausesValidation="false"
                                Visible='<%# Eval("DataFim") == DBNull.Value %>'>Encerrar</asp:LinkButton>
                            <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                CommandName="Excluir" CommandArgument='<%# Eval("OrientacaoId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Excluir este registro de orientação?');">Excluir</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="grid-pager" />
                <EmptyDataRowStyle CssClass="text-muted" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
