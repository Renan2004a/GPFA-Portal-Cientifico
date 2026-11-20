<%@ Page Title="Projetos" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Projetos.aspx.cs" Inherits="GPFA.Portal.Admin.ProjetosAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminTopbarActions" runat="server">
    <asp:Button ID="btnNovoTopo" runat="server" Text="Novo projeto" CssClass="btn-gpfa btn-brand btn-sm js-submit-once"
        OnClick="btnNovo_Click" CausesValidation="false" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Projetos de pesquisa</h2>
            <p>Coordenação única, participações e vínculos de estudantes. <asp:Literal ID="litTotal" runat="server" /></p>
        </div>
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2><asp:Literal ID="litTituloForm" runat="server" /></h2>
                <p>Cada projeto possui um único coordenador responsável (RN03). O coordenador é participante implícito.</p>
            </div>
            <asp:Button ID="btnFecharForm" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>

        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
        <asp:ValidationSummary ID="vsProjeto" runat="server" CssClass="validation-summary"
            HeaderText="Corrija os campos destacados:" ValidationGroup="proj" />

        <div class="form-grid">
            <div class="col-9">
                <label class="form-label" for="<%= txtTitulo.ClientID %>">Título <span class="req">*</span></label>
                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="200" ValidationGroup="proj" />
                <asp:RequiredFieldValidator ID="rfvTitulo" runat="server" ControlToValidate="txtTitulo" CssClass="field-error"
                    ErrorMessage="Informe o título." ValidationGroup="proj" Display="Dynamic" />
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= txtSigla.ClientID %>">Sigla</label>
                <asp:TextBox ID="txtSigla" runat="server" CssClass="form-control" MaxLength="30" placeholder="Ex.: MQ-2D" />
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= txtResumo.ClientID %>">Resumo público <span class="req">*</span></label>
                <asp:TextBox ID="txtResumo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="1000" ValidationGroup="proj" />
                <asp:RequiredFieldValidator ID="rfvResumo" runat="server" ControlToValidate="txtResumo" CssClass="field-error"
                    ErrorMessage="Informe o resumo." ValidationGroup="proj" Display="Dynamic" />
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= txtDescricao.ClientID %>">Descrição completa</label>
                <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= ddlCoordenador.ClientID %>">Coordenador <span class="req">*</span></label>
                <asp:DropDownList ID="ddlCoordenador" runat="server" CssClass="form-select" ValidationGroup="proj" />
                <asp:RequiredFieldValidator ID="rfvCoordenador" runat="server" ControlToValidate="ddlCoordenador" CssClass="field-error"
                    ErrorMessage="Selecione o coordenador." InitialValue="" ValidationGroup="proj" Display="Dynamic" />
                <asp:Panel ID="pnlCoordFixo" runat="server" Visible="false" CssClass="form-text">
                    Coordenadores só podem criar/editar projetos sob sua própria coordenação.
                </asp:Panel>
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= ddlSituacao.ClientID %>">Situação <span class="req">*</span></label>
                <asp:DropDownList ID="ddlSituacao" runat="server" CssClass="form-select" ValidationGroup="proj" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtLinha.ClientID %>">Linha de pesquisa</label>
                <asp:TextBox ID="txtLinha" runat="server" CssClass="form-control" MaxLength="120" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtInicio.ClientID %>">Início <span class="req">*</span></label>
                <asp:TextBox ID="txtInicio" runat="server" CssClass="form-control" TextMode="Date" ValidationGroup="proj" />
                <asp:RequiredFieldValidator ID="rfvInicio" runat="server" ControlToValidate="txtInicio" CssClass="field-error"
                    ErrorMessage="Informe o início." ValidationGroup="proj" Display="Dynamic" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtFimPrevisto.ClientID %>">Término previsto</label>
                <asp:TextBox ID="txtFimPrevisto" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtFim.ClientID %>">Término realizado</label>
                <asp:TextBox ID="txtFim" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-8">
                <label class="form-label" for="<%= txtImagem.ClientID %>">Imagem de capa (URL ou caminho local)</label>
                <asp:TextBox ID="txtImagem" runat="server" CssClass="form-control" MaxLength="300" placeholder="/Content/img/projetos/nome.svg" />
            </div>
            <div class="col-4 d-flex align-items-end">
                <asp:CheckBox ID="chkDestaque" runat="server" CssClass="form-check-input me-2" />
                <label class="form-label d-inline mb-2" for="<%= chkDestaque.ClientID %>">Exibir na página inicial</label>
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar projeto" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnSalvar_Click" ValidationGroup="proj" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn-gpfa btn-outline-brand"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div class="table-shell">
        <div class="table-responsive">
            <asp:GridView ID="gvProjetos" runat="server" AutoGenerateColumns="false" DataKeyNames="ProjetoId"
                CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                AllowPaging="true" PageSize="10" OnPageIndexChanging="gvProjetos_PageIndexChanging"
                OnRowCommand="gvProjetos_RowCommand" EmptyDataText="Nenhum projeto cadastrado.">
                <Columns>
                    <asp:TemplateField HeaderText="Projeto">
                        <ItemTemplate>
                            <strong><%#: Eval("Titulo") %></strong><br />
                            <span class="tiny muted"><%#: Eval("Sigla") %> · <%#: Eval("LinhaPesquisa") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CoordenadorNome" HeaderText="Coordenador" />
                    <asp:TemplateField HeaderText="Situação">
                        <ItemTemplate><span class='status-pill <%# Eval("SituacaoCss") %>'><%#: Eval("Situacao") %></span></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="PeriodoFormatado" HeaderText="Período" />
                    <asp:BoundField DataField="TotalCaptado" HeaderText="Verba" DataFormatString="{0:C0}" ItemStyle-CssClass="nowrap" />
                    <asp:TemplateField HeaderText="Equipe" ItemStyle-CssClass="text-center">
                        <ItemTemplate><%#: Eval("QtdPesquisadores") %> pesq. / <%#: Eval("QtdEstudantes") %> est.</ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEditar" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                CommandName="Editar" CommandArgument='<%# Eval("ProjetoId") %>' CausesValidation="false">Editar</asp:LinkButton>
                            <a class="btn-gpfa btn-outline-brand btn-sm" href='<%# ResolveUrl("~/Admin/Participacoes.aspx") + "?projeto=" + Eval("ProjetoId") %>'>Equipe</a>
                            <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                CommandName="Excluir" CommandArgument='<%# Eval("ProjetoId") %>' CausesValidation="false"
                                Visible='<%# EhAdministrador %>'
                                OnClientClick="return confirm('Excluir este projeto? A operação é bloqueada se houver vínculos.');">Excluir</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="grid-pager" />
                <EmptyDataRowStyle CssClass="text-muted" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
