<%@ Page Title="Usuários e permissões" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="GPFA.Portal.Admin.UsuariosAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Usuários e permissões</h2>
            <p>Perfis: Administrador (total), Coordenador (projetos próprios) e Pesquisador (consulta e perfil próprio).</p>
        </div>
        <asp:Button ID="btnNovo" runat="server" Text="Novo usuário" CssClass="btn-gpfa btn-brand js-submit-once"
            OnClick="btnNovo_Click" CausesValidation="false" />
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2><asp:Literal ID="litTituloForm" runat="server" /></h2>
                <p>Senhas são armazenadas com PBKDF2-HMAC-SHA256 e nunca em texto puro.</p>
            </div>
            <asp:Button ID="btnFecharForm" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>

        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
        <asp:ValidationSummary ID="vsUsuario" runat="server" CssClass="validation-summary"
            HeaderText="Corrija os campos destacados:" ValidationGroup="usr" />

        <div class="form-grid">
            <div class="col-4">
                <label class="form-label" for="<%= txtLogin.ClientID %>">Login <span class="req">*</span></label>
                <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" MaxLength="30" ValidationGroup="usr" />
                <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ControlToValidate="txtLogin" CssClass="field-error"
                    ErrorMessage="Informe o login." ValidationGroup="usr" Display="Dynamic" />
            </div>
            <div class="col-5">
                <label class="form-label" for="<%= txtNome.ClientID %>">Nome completo <span class="req">*</span></label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="150" ValidationGroup="usr" />
                <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome" CssClass="field-error"
                    ErrorMessage="Informe o nome." ValidationGroup="usr" Display="Dynamic" />
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= txtEmail.ClientID %>">E-mail <span class="req">*</span></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="150" TextMode="Email" ValidationGroup="usr" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" CssClass="field-error"
                    ErrorMessage="Informe o e-mail." ValidationGroup="usr" Display="Dynamic" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= ddlPerfil.ClientID %>">Perfil <span class="req">*</span></label>
                <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-select" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlPerfil_SelectedIndexChanged" />
            </div>
            <div class="col-8">
                <label class="form-label" for="<%= ddlPesquisador.ClientID %>">Pesquisador vinculado</label>
                <asp:DropDownList ID="ddlPesquisador" runat="server" CssClass="form-select" />
                <div class="form-text">Obrigatório para Coordenador e Pesquisador. Administradores podem não ter vínculo.</div>
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtSenha.ClientID %>">Senha</label>
                <asp:TextBox ID="txtSenha" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" />
                <div class="form-text">Obrigatória no cadastro. Em branco mantém a senha atual. Mínimo 8 caracteres com maiúscula, minúscula e número.</div>
            </div>
            <div class="col-6 d-flex align-items-end pb-2">
                <asp:CheckBox ID="chkAtivo" runat="server" CssClass="form-check-input me-2" Checked="true" />
                <label class="form-label d-inline mb-0" for="<%= chkAtivo.ClientID %>">Usuário ativo</label>
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar usuário" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnSalvar_Click" ValidationGroup="usr" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn-gpfa btn-outline-brand"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div class="table-shell">
        <div class="table-responsive">
            <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="false" DataKeyNames="UsuarioId"
                CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                OnRowCommand="gvUsuarios_RowCommand" EmptyDataText="Nenhum usuário cadastrado.">
                <Columns>
                    <asp:TemplateField HeaderText="Usuário">
                        <ItemTemplate><strong><%#: Eval("Login") %></strong><br /><span class="tiny muted"><%#: Eval("NomeCompleto") %></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Email" HeaderText="E-mail" />
                    <asp:BoundField DataField="PerfilNome" HeaderText="Perfil" />
                    <asp:BoundField DataField="PesquisadorNome" HeaderText="Pesquisador" NullDisplayText="—" />
                    <asp:BoundField DataField="UltimoAcesso" HeaderText="Último acesso" DataFormatString="{0:dd/MM/yyyy HH:mm}" NullDisplayText="Nunca" />
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
                                CommandName="Editar" CommandArgument='<%# Eval("UsuarioId") %>' CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                CommandName="Excluir" CommandArgument='<%# Eval("UsuarioId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Excluir este usuário? O último administrador ativo não pode ser removido.');">Excluir</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataRowStyle CssClass="text-muted" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
