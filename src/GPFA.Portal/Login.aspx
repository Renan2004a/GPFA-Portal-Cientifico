<%@ Page Title="Entrar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GPFA.Portal.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-wrap">
        <div class="auth-card">
            <a class="brand" href="<%= ResolveUrl("~/Default.aspx") %>">
                <svg class="brand-mark" viewBox="0 0 48 48" aria-hidden="true">
                    <circle cx="24" cy="24" r="23" fill="#0e2f46"/>
                    <ellipse cx="24" cy="24" rx="19" ry="8.6" fill="none" stroke="#2ec4b6" stroke-width="1.1" opacity=".8" transform="rotate(-26 24 24)"/>
                    <ellipse cx="24" cy="24" rx="19" ry="8.6" fill="none" stroke="#e9a23b" stroke-width="1" opacity=".55" transform="rotate(34 24 24)"/>
                    <circle cx="24" cy="20.4" r="3" fill="#f2fbf9"/><circle cx="24" cy="25.4" r="4.2" fill="#2ec4b6"/><circle cx="24" cy="31.6" r="5.2" fill="#e9a23b"/>
                </svg>
                <span class="brand-copy"><span class="brand-name">GPFA</span><span class="brand-sub">Área restrita</span></span>
            </a>

            <h1>Acessar o painel</h1>
            <p class="auth-sub">Entre com suas credenciais institucionais para gerenciar pessoas, projetos e financiamentos.</p>

            <asp:Panel ID="pnlErro" runat="server" Visible="false" CssClass="alert-gpfa alert-danger-gpfa" role="alert">
                <asp:Literal ID="litErro" runat="server" />
            </asp:Panel>

            <asp:ValidationSummary ID="vsLogin" runat="server" CssClass="validation-summary"
                HeaderText="Verifique os campos:" ValidationGroup="login" />

            <div class="mb-3">
                <label class="form-label" for="<%= txtLogin.ClientID %>">Login</label>
                <asp:TextBox ID="txtLogin" runat="server" CssClass="form-control" MaxLength="30"
                    autocomplete="username" placeholder="seu.login" ValidationGroup="login" />
                <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ControlToValidate="txtLogin"
                    CssClass="field-error" ErrorMessage="Informe o login." ValidationGroup="login" Display="Dynamic" />
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%= txtSenha.ClientID %>">Senha</label>
                <asp:TextBox ID="txtSenha" runat="server" CssClass="form-control" TextMode="Password"
                    autocomplete="current-password" placeholder="Sua senha" ValidationGroup="login" />
                <asp:RequiredFieldValidator ID="rfvSenha" runat="server" ControlToValidate="txtSenha"
                    CssClass="field-error" ErrorMessage="Informe a senha." ValidationGroup="login" Display="Dynamic" />
            </div>

            <asp:Button ID="btnEntrar" runat="server" Text="Entrar no painel" CssClass="btn-gpfa btn-brand w-100 js-submit-once"
                OnClick="btnEntrar_Click" ValidationGroup="login" />

            <div class="d-flex justify-content-between mt-3">
                <a class="tiny" href="<%= ResolveUrl("~/Default.aspx") %>">← Voltar ao portal</a>
                <span class="tiny muted">Sessão segura · 60 min</span>
            </div>

            <asp:Panel ID="pnlDemo" runat="server" Visible="false" CssClass="auth-demo">
                <strong>Ambiente de demonstração</strong><br />
                Administrador: <code>admin</code> / <code>Gpfa@2026</code><br />
                Coordenador: <code>coordenador</code> / <code>Coord@2026</code><br />
                Pesquisador: <code>pesquisador</code> / <code>Pesq@2026</code>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
