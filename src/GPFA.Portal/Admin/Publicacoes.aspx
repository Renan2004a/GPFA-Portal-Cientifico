<%@ Page Title="Publicações" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Publicacoes.aspx.cs" Inherits="GPFA.Portal.Admin.PublicacoesAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Publicações científicas</h2>
            <p>Artigos, capítulos e patentes com autoria de pesquisadores do grupo. <asp:Literal ID="litTotal" runat="server" /></p>
        </div>
        <asp:Button ID="btnNova" runat="server" Text="Nova publicação" CssClass="btn-gpfa btn-brand js-submit-once"
            OnClick="btnNova_Click" CausesValidation="false" />
    </div>

    <div class="filter-bar">
        <div class="filter-field" style="flex:3">
            <label for="<%= txtBusca.ClientID %>">Buscar</label>
            <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Título, veículo ou autor" />
        </div>
        <div class="filter-actions">
            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnFiltrar_Click" CausesValidation="false" />
            <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Admin/Publicacoes.aspx") %>">Limpar</a>
        </div>
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2><asp:Literal ID="litTituloForm" runat="server" /></h2>
                <p>Toda publicação deve ter ao menos um autor pesquisador e um DOI ou link de acesso.</p>
            </div>
            <asp:Button ID="btnFecharForm" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>

        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
        <asp:ValidationSummary ID="vsPublicacao" runat="server" CssClass="validation-summary"
            HeaderText="Corrija os campos destacados:" ValidationGroup="pub" />

        <div class="form-grid">
            <div class="col-12">
                <label class="form-label" for="<%= txtTitulo.ClientID %>">Título <span class="req">*</span></label>
                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="300" ValidationGroup="pub" />
                <asp:RequiredFieldValidator ID="rfvTitulo" runat="server" ControlToValidate="txtTitulo" CssClass="field-error"
                    ErrorMessage="Informe o título." ValidationGroup="pub" Display="Dynamic" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtVeiculo.ClientID %>">Veículo de publicação <span class="req">*</span></label>
                <asp:TextBox ID="txtVeiculo" runat="server" CssClass="form-control" MaxLength="200" ValidationGroup="pub" />
                <asp:RequiredFieldValidator ID="rfvVeiculo" runat="server" ControlToValidate="txtVeiculo" CssClass="field-error"
                    ErrorMessage="Informe o veículo." ValidationGroup="pub" Display="Dynamic" />
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= txtAno.ClientID %>">Ano <span class="req">*</span></label>
                <asp:TextBox ID="txtAno" runat="server" CssClass="form-control" TextMode="Number" ValidationGroup="pub" />
                <asp:RequiredFieldValidator ID="rfvAno" runat="server" ControlToValidate="txtAno" CssClass="field-error"
                    ErrorMessage="Informe o ano." ValidationGroup="pub" Display="Dynamic" />
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= ddlTipo.ClientID %>">Tipo</label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtDoi.ClientID %>">DOI</label>
                <asp:TextBox ID="txtDoi" runat="server" CssClass="form-control" MaxLength="150" placeholder="10.xxxx/xxxxx" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtLink.ClientID %>">Link de acesso</label>
                <asp:TextBox ID="txtLink" runat="server" CssClass="form-control" MaxLength="400" placeholder="https://..." />
                <div class="form-text">Informe ao menos DOI ou link de acesso (regra RN05).</div>
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= ddlProjeto.ClientID %>">Projeto associado</label>
                <asp:DropDownList ID="ddlProjeto" runat="server" CssClass="form-select" />
            </div>
            <div class="col-12">
                <label class="form-label">Autores pesquisadores <span class="req">*</span></label>
                <asp:CheckBoxList ID="cblAutores" runat="server" CssClass="proj-check-list" RepeatLayout="Flow" />
                <asp:CustomValidator ID="cvAutores" runat="server" CssClass="field-error" ValidationGroup="pub"
                    ErrorMessage="Selecione ao menos um autor." OnServerValidate="cvAutores_ServerValidate" Display="Dynamic" />
                <div class="form-text">A ordem de autoria segue a ordem de cadastro dos pesquisadores; o primeiro autor é destacado no perfil.</div>
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar publicação" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnSalvar_Click" ValidationGroup="pub" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn-gpfa btn-outline-brand"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div class="table-shell">
        <div class="table-responsive">
            <asp:GridView ID="gvPublicacoes" runat="server" AutoGenerateColumns="false" DataKeyNames="PublicacaoId"
                CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                AllowPaging="true" PageSize="10" OnPageIndexChanging="gvPublicacoes_PageIndexChanging"
                OnRowCommand="gvPublicacoes_RowCommand" EmptyDataText="Nenhuma publicação cadastrada.">
                <Columns>
                    <asp:TemplateField HeaderText="Publicação">
                        <ItemTemplate>
                            <strong><%#: Eval("Titulo") %></strong><br />
                            <span class="tiny muted"><%#: Eval("Autores") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Veiculo" HeaderText="Veículo" />
                    <asp:BoundField DataField="Ano" HeaderText="Ano" ItemStyle-CssClass="text-center" />
                    <asp:BoundField DataField="ProjetoSigla" HeaderText="Projeto" NullDisplayText="—" />
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                        <ItemTemplate>
                            <a class="btn-gpfa btn-outline-brand btn-sm" href='<%# ResolveUrl("~/Publicacoes.aspx") + "?q=" + Server.UrlEncode(Convert.ToString(Eval("Titulo"))) %>'>Ver</a>
                            <asp:LinkButton ID="lnkEditar" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                CommandName="Editar" CommandArgument='<%# Eval("PublicacaoId") %>' CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                CommandName="Excluir" CommandArgument='<%# Eval("PublicacaoId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Excluir esta publicação e seus destaques associados?');">Excluir</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="grid-pager" />
                <EmptyDataRowStyle CssClass="text-muted" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
