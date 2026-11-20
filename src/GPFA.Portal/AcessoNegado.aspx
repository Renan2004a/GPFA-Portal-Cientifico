<%@ Page Title="Acesso negado" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AcessoNegado.aspx.cs" Inherits="GPFA.Portal.AcessoNegado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-wrap">
        <div class="auth-card text-center" style="width:min(100%,560px)">
            <div class="empty-icon" style="margin:0 auto 1.2rem;width:64px;height:64px">
                <svg class="icon icon-lg" aria-hidden="true"><use href="#i-shield"/></svg>
            </div>
            <div class="eyebrow" style="justify-content:center">Erro 403</div>
            <h1>Acesso não autorizado</h1>
            <p class="auth-sub">Seu perfil não possui permissão para acessar esta área. Se acredita que isso é um engano, procure o administrador do portal.</p>
            <div class="d-flex justify-content-center gap-2 flex-wrap">
                <a class="btn-gpfa btn-brand" href="<%= ResolveUrl("~/Admin/Default.aspx") %>">Ir ao painel</a>
                <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Default.aspx") %>">Portal público</a>
            </div>
        </div>
    </div>
</asp:Content>
