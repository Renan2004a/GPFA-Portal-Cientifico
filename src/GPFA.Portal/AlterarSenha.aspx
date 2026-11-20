<%@ Page Title="Alterar senha" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AlterarSenha.aspx.cs" Inherits="GPFA.Portal.AlterarSenha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-wrap">
        <div class="auth-card" style="width:min(100%,520px)">
            <div class="eyebrow">Segurança</div>
            <h1>Alterar senha</h1>
            <p class="auth-sub">Use ao menos 8 caracteres, com letras maiúsculas, minúsculas e números.</p>

            <asp:ValidationSummary ID="vsSenha" runat="server" CssClass="validation-summary"
                HeaderText="Verifique os campos:" ValidationGroup="senha" />

            <div class="mb-3">
                <label class="form-label" for="<%= txtAtual.ClientID %>">Senha atual</label>
                <asp:TextBox ID="txtAtual" runat="server" CssClass="form-control" TextMode="Password" autocomplete="current-password" ValidationGroup="senha" />
                <asp:RequiredFieldValidator ID="rfvAtual" runat="server" ControlToValidate="txtAtual" CssClass="field-error"
                    ErrorMessage="Informe a senha atual." ValidationGroup="senha" Display="Dynamic" />
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%= txtNova.ClientID %>">Nova senha</label>
                <asp:TextBox ID="txtNova" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" ValidationGroup="senha" />
                <asp:RequiredFieldValidator ID="rfvNova" runat="server" ControlToValidate="txtNova" CssClass="field-error"
                    ErrorMessage="Informe a nova senha." ValidationGroup="senha" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revNova" runat="server" ControlToValidate="txtNova" CssClass="field-error"
                    ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}$"
                    ErrorMessage="A senha deve ter 8+ caracteres, com maiúscula, minúscula e número."
                    ValidationGroup="senha" Display="Dynamic" />
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%= txtConfirmacao.ClientID %>">Confirmar nova senha</label>
                <asp:TextBox ID="txtConfirmacao" runat="server" CssClass="form-control" TextMode="Password" autocomplete="new-password" ValidationGroup="senha" />
                <asp:RequiredFieldValidator ID="rfvConfirmacao" runat="server" ControlToValidate="txtConfirmacao" CssClass="field-error"
                    ErrorMessage="Confirme a nova senha." ValidationGroup="senha" Display="Dynamic" />
                <asp:CompareValidator ID="cvSenha" runat="server" ControlToValidate="txtConfirmacao" ControlToCompare="txtNova"
                    CssClass="field-error" ErrorMessage="A confirmação não coincide com a nova senha." ValidationGroup="senha" Display="Dynamic" />
            </div>

            <div class="d-flex gap-2">
                <asp:Button ID="btnAlterar" runat="server" Text="Salvar nova senha" CssClass="btn-gpfa btn-brand js-submit-once"
                    OnClick="btnAlterar_Click" ValidationGroup="senha" />
                <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Admin/Default.aspx") %>">Cancelar</a>
            </div>
        </div>
    </div>
</asp:Content>
