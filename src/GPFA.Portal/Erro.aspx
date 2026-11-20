<%@ Page Title="Não foi possível concluir" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Erro.aspx.cs" Inherits="GPFA.Portal.Erro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-wrap">
        <div class="auth-card text-center" style="width:min(100%,560px)">
            <div class="empty-icon" style="margin:0 auto 1.2rem;width:64px;height:64px">
                <svg class="icon icon-lg" aria-hidden="true"><use href="#i-alert"/></svg>
            </div>
            <div class="eyebrow" style="justify-content:center">Erro inesperado</div>
            <h1>Algo saiu do curso</h1>
            <p class="auth-sub">Não foi possível concluir esta operação. O erro foi registrado para análise da equipe. Tente novamente ou volte ao portal.</p>
            <div class="d-flex justify-content-center gap-2 flex-wrap">
                <a class="btn-gpfa btn-brand" href="<%= ResolveUrl("~/Default.aspx") %>">Voltar ao início</a>
                <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Projetos.aspx") %>">Ver projetos</a>
            </div>
        </div>
    </div>
</asp:Content>
