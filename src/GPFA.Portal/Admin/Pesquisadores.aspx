<%@ Page Title="Pesquisadores" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Pesquisadores.aspx.cs" Inherits="GPFA.Portal.Admin.PesquisadoresAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Pesquisadores</h2>
            <p>Diretório acadêmico, links externos e mini-biografias. <asp:Literal ID="litTotal" runat="server" /></p>
        </div>
        <asp:Button ID="btnNovo" runat="server" Text="Novo pesquisador" CssClass="btn-gpfa btn-brand js-submit-once"
            OnClick="btnNovo_Click" CausesValidation="false" />
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2><asp:Literal ID="litTituloForm" runat="server" /></h2>
                <p>Dados acadêmicos e de divulgação. Campos com <span class="req">*</span> são obrigatórios.</p>
            </div>
            <asp:Button ID="btnCancelarTopo" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>

        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
        <asp:ValidationSummary ID="vsPesquisador" runat="server" CssClass="validation-summary"
            HeaderText="Corrija os campos destacados:" ValidationGroup="pesq" />

        <div class="form-grid">
            <div class="col-8">
                <label class="form-label" for="<%= txtNome.ClientID %>">Nome completo <span class="req">*</span></label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="150" ValidationGroup="pesq" />
                <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome" CssClass="field-error"
                    ErrorMessage="Informe o nome completo." ValidationGroup="pesq" Display="Dynamic" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtEmail.ClientID %>">E-mail <span class="req">*</span></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="150" TextMode="Email" ValidationGroup="pesq" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" CssClass="field-error"
                    ErrorMessage="Informe o e-mail." ValidationGroup="pesq" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" CssClass="field-error"
                    ValidationExpression="^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"
                    ErrorMessage="E-mail inválido." ValidationGroup="pesq" Display="Dynamic" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtFormacao.ClientID %>">Formação acadêmica <span class="req">*</span></label>
                <asp:TextBox ID="txtFormacao" runat="server" CssClass="form-control" MaxLength="200" ValidationGroup="pesq" />
                <asp:RequiredFieldValidator ID="rfvFormacao" runat="server" ControlToValidate="txtFormacao" CssClass="field-error"
                    ErrorMessage="Informe a formação." ValidationGroup="pesq" Display="Dynamic" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtArea.ClientID %>">Área de atuação <span class="req">*</span></label>
                <asp:TextBox ID="txtArea" runat="server" CssClass="form-control" MaxLength="150" ValidationGroup="pesq" />
                <asp:RequiredFieldValidator ID="rfvArea" runat="server" ControlToValidate="txtArea" CssClass="field-error"
                    ErrorMessage="Informe a área." ValidationGroup="pesq" Display="Dynamic" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtTitulacao.ClientID %>">Titulação</label>
                <asp:TextBox ID="txtTitulacao" runat="server" CssClass="form-control" MaxLength="80" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtDepartamento.ClientID %>">Departamento</label>
                <asp:TextBox ID="txtDepartamento" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtLinha.ClientID %>">Linha de pesquisa</label>
                <asp:TextBox ID="txtLinha" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtInstituicao.ClientID %>">Instituição</label>
                <asp:TextBox ID="txtInstituicao" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtFoto.ClientID %>">Foto (URL ou caminho local)</label>
                <asp:TextBox ID="txtFoto" runat="server" CssClass="form-control" MaxLength="300" placeholder="/Content/img/avatars/nome.svg" />
                <div class="form-text">Use uma imagem quadrada. O portal exibe iniciais quando não há foto.</div>
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= txtBio.ClientID %>">Mini-biografia</label>
                <asp:TextBox ID="txtBio" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="4000" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtScholar.ClientID %>">Google Scholar</label>
                <asp:TextBox ID="txtScholar" runat="server" CssClass="form-control" MaxLength="300" placeholder="https://..." />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtOrcid.ClientID %>">ORCID</label>
                <asp:TextBox ID="txtOrcid" runat="server" CssClass="form-control" MaxLength="100" placeholder="https://orcid.org/..." />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtLattes.ClientID %>">Currículo Lattes</label>
                <asp:TextBox ID="txtLattes" runat="server" CssClass="form-control" MaxLength="300" placeholder="http://lattes.cnpq.br/..." />
            </div>
            <div class="col-12">
                <asp:CheckBox ID="chkAtivo" runat="server" CssClass="form-check-input me-2" Checked="true" />
                <label class="form-label d-inline" for="<%= chkAtivo.ClientID %>">Perfil ativo e visível no diretório público</label>
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar pesquisador" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnSalvar_Click" ValidationGroup="pesq" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn-gpfa btn-outline-brand"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div class="table-shell">
        <div class="table-responsive">
            <asp:GridView ID="gvPesquisadores" runat="server" AutoGenerateColumns="false" DataKeyNames="PesquisadorId"
                CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                AllowPaging="true" PageSize="10" OnPageIndexChanging="gvPesquisadores_PageIndexChanging"
                OnRowCommand="gvPesquisadores_RowCommand" EmptyDataText="Nenhum pesquisador cadastrado.">
                <Columns>
                    <asp:TemplateField HeaderText="Pesquisador">
                        <ItemTemplate>
                            <div class="d-flex align-items-center gap-2">
                                <%# AvatarHtml(Eval("FotoUrl"), Eval("NomeCompleto"), "avatar avatar-sm") %>
                                <div>
                                    <strong><%#: Eval("NomeCompleto") %></strong><br />
                                    <span class="tiny muted"><%#: Eval("Email") %></span>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="AreaAtuacao" HeaderText="Área" />
                    <asp:BoundField DataField="QtdProjetos" HeaderText="Projetos" ItemStyle-CssClass="text-center" />
                    <asp:BoundField DataField="QtdOrientandosAtuais" HeaderText="Orientandos" ItemStyle-CssClass="text-center" />
                    <asp:BoundField DataField="TotalCaptado" HeaderText="Verba" DataFormatString="{0:C0}" ItemStyle-CssClass="nowrap" />
                    <asp:TemplateField HeaderText="Situação">
                        <ItemTemplate>
                            <span class='status-pill <%# Convert.ToBoolean(Eval("Ativo")) ? "status-andamento" : "status-neutro" %>'>
                                <%# Convert.ToBoolean(Eval("Ativo")) ? "Ativo" : "Inativo" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEditar" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                CommandName="Editar" CommandArgument='<%# Eval("PesquisadorId") %>' CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                CommandName="Excluir" CommandArgument='<%# Eval("PesquisadorId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Excluir este pesquisador? A ação será bloqueada se houver vínculos.');">Excluir</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="grid-pager" />
                <EmptyDataRowStyle CssClass="text-muted" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
