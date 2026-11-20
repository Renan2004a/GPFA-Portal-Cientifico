<%@ Page Title="Estudantes" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Estudantes.aspx.cs" Inherits="GPFA.Portal.Admin.EstudantesAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Estudantes</h2>
            <p>Todo estudante ativo deve ter orientador principal e vínculo com ao menos um projeto. <asp:Literal ID="litTotal" runat="server" /></p>
        </div>
        <asp:Button ID="btnNovo" runat="server" Text="Novo estudante" CssClass="btn-gpfa btn-brand js-submit-once"
            OnClick="btnNovo_Click" CausesValidation="false" />
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="form-panel mb-4">
        <div class="form-panel-head">
            <div>
                <h2><asp:Literal ID="litTituloForm" runat="server" /></h2>
                <p>O cadastro cria, na mesma transação, o estudante, a orientação principal e os vínculos com projetos.</p>
            </div>
            <asp:Button ID="btnCancelarTopo" runat="server" Text="Fechar" CssClass="btn-gpfa btn-outline-brand btn-sm"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>

        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
        <asp:ValidationSummary ID="vsEstudante" runat="server" CssClass="validation-summary"
            HeaderText="Corrija os campos destacados:" ValidationGroup="est" />

        <div class="form-grid">
            <div class="col-8">
                <label class="form-label" for="<%= txtNome.ClientID %>">Nome completo <span class="req">*</span></label>
                <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" MaxLength="150" ValidationGroup="est" />
                <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome" CssClass="field-error"
                    ErrorMessage="Informe o nome completo." ValidationGroup="est" Display="Dynamic" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= txtEmail.ClientID %>">E-mail <span class="req">*</span></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="150" TextMode="Email" ValidationGroup="est" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" CssClass="field-error"
                    ErrorMessage="Informe o e-mail." ValidationGroup="est" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" CssClass="field-error"
                    ValidationExpression="^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"
                    ErrorMessage="E-mail inválido." ValidationGroup="est" Display="Dynamic" />
            </div>
            <div class="col-4">
                <label class="form-label" for="<%= ddlNivel.ClientID %>">Nível acadêmico <span class="req">*</span></label>
                <asp:DropDownList ID="ddlNivel" runat="server" CssClass="form-select" ValidationGroup="est" />
            </div>
            <div class="col-5">
                <label class="form-label" for="<%= txtCurso.ClientID %>">Curso <span class="req">*</span></label>
                <asp:TextBox ID="txtCurso" runat="server" CssClass="form-control" MaxLength="120" ValidationGroup="est" />
                <asp:RequiredFieldValidator ID="rfvCurso" runat="server" ControlToValidate="txtCurso" CssClass="field-error"
                    ErrorMessage="Informe o curso." ValidationGroup="est" Display="Dynamic" />
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= txtIngresso.ClientID %>">Ingresso <span class="req">*</span></label>
                <asp:TextBox ID="txtIngresso" runat="server" CssClass="form-control" TextMode="Date" ValidationGroup="est" />
                <asp:RequiredFieldValidator ID="rfvIngresso" runat="server" ControlToValidate="txtIngresso" CssClass="field-error"
                    ErrorMessage="Informe o ingresso." ValidationGroup="est" Display="Dynamic" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= ddlOrientador.ClientID %>">Orientador principal <span class="req">*</span></label>
                <asp:DropDownList ID="ddlOrientador" runat="server" CssClass="form-select" ValidationGroup="est" />
                <asp:RequiredFieldValidator ID="rfvOrientador" runat="server" ControlToValidate="ddlOrientador" CssClass="field-error"
                    ErrorMessage="Selecione o orientador principal." ValidationGroup="est" InitialValue="" Display="Dynamic" />
                <div class="form-text">Somente pesquisadores ativos do grupo (RN04).</div>
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= ddlSituacao.ClientID %>">Situação</label>
                <asp:DropDownList ID="ddlSituacao" runat="server" CssClass="form-select" />
                <div class="form-text">Novos cadastros iniciam como Ativo.</div>
            </div>
            <div class="col-3">
                <label class="form-label" for="<%= txtConclusao.ClientID %>">Conclusão</label>
                <asp:TextBox ID="txtConclusao" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtFormacao.ClientID %>">Formação (texto exibido)</label>
                <asp:TextBox ID="txtFormacao" runat="server" CssClass="form-control" MaxLength="200" placeholder="Ex.: Mestrado em Física (em curso)" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtArea.ClientID %>">Área de atuação</label>
                <asp:TextBox ID="txtArea" runat="server" CssClass="form-control" MaxLength="150" />
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= txtCoorientadores.ClientID %>">Coorientadores externos / de outros departamentos</label>
                <asp:TextBox ID="txtCoorientadores" runat="server" CssClass="form-control" MaxLength="500"
                    placeholder="Nomes livres, separados por ponto e vírgula. Não precisam ser pesquisadores do GPFA." />
                <div class="form-text">Coorientadores externos são registro textual; coorientadores internos são gerenciados em Orientações.</div>
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtFoto.ClientID %>">Foto (URL ou caminho local)</label>
                <asp:TextBox ID="txtFoto" runat="server" CssClass="form-control" MaxLength="300" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtTipoVinculo.ClientID %>">Tipo de vínculo com projeto</label>
                <asp:TextBox ID="txtTipoVinculo" runat="server" CssClass="form-control" MaxLength="60" placeholder="Ex.: Bolsa de mestrado" />
            </div>
            <div class="col-12">
                <label class="form-label" for="<%= txtBio.ClientID %>">Mini-biografia</label>
                <asp:TextBox ID="txtBio" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="4000" />
            </div>
            <div class="col-12">
                <label class="form-label">Projetos vinculados <span class="req">*</span> (RN01)</label>
                <asp:CheckBoxList ID="cblProjetos" runat="server" CssClass="proj-check-list" RepeatLayout="Flow" />
                <asp:CustomValidator ID="cvProjetos" runat="server" CssClass="field-error" ValidationGroup="est"
                    ErrorMessage="Selecione ao menos um projeto de pesquisa." OnServerValidate="cvProjetos_ServerValidate"
                    Display="Dynamic" />
                <asp:Panel ID="pnlProjetosEdicao" runat="server" Visible="false" CssClass="form-text mt-2">
                    Os vínculos com projetos de um estudante já cadastrado são gerenciados na página <strong>Projetos → Vínculos de estudantes</strong>.
                </asp:Panel>
            </div>
            <div class="col-12">
                <asp:CheckBox ID="chkAtivo" runat="server" CssClass="form-check-input me-2" Checked="true" />
                <label class="form-label d-inline" for="<%= chkAtivo.ClientID %>">Perfil ativo no portal</label>
            </div>
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSalvar" runat="server" Text="Salvar estudante" CssClass="btn-gpfa btn-brand js-submit-once"
                OnClick="btnSalvar_Click" ValidationGroup="est" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn-gpfa btn-outline-brand"
                OnClick="btnCancelar_Click" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div class="table-shell">
        <div class="table-responsive">
            <asp:GridView ID="gvEstudantes" runat="server" AutoGenerateColumns="false" DataKeyNames="EstudanteId"
                CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                AllowPaging="true" PageSize="10" OnPageIndexChanging="gvEstudantes_PageIndexChanging"
                OnRowCommand="gvEstudantes_RowCommand" EmptyDataText="Nenhum estudante cadastrado.">
                <Columns>
                    <asp:TemplateField HeaderText="Estudante">
                        <ItemTemplate>
                            <div class="d-flex align-items-center gap-2">
                                <%# AvatarHtml(Eval("FotoUrl"), Eval("NomeCompleto"), "avatar avatar-sm") %>
                                <div>
                                    <strong><%#: Eval("NomeCompleto") %></strong><br />
                                    <span class="tiny muted"><%#: Eval("NivelAcademico") %> em <%#: Eval("Curso") %></span>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="OrientadorNome" HeaderText="Orientador principal" />
                    <asp:BoundField DataField="QtdProjetosAtivos" HeaderText="Projetos" ItemStyle-CssClass="text-center" />
                    <asp:BoundField DataField="DataIngresso" HeaderText="Ingresso" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField HeaderText="Situação">
                        <ItemTemplate>
                            <span class='status-pill <%# Eval("SituacaoCss") %>'><%#: Eval("Situacao") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEditar" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                CommandName="Editar" CommandArgument='<%# Eval("EstudanteId") %>' CausesValidation="false">Editar</asp:LinkButton>
                            <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                CommandName="Excluir" CommandArgument='<%# Eval("EstudanteId") %>' CausesValidation="false"
                                OnClientClick="return confirm('Excluir definitivamente este estudante e seu histórico de orientações/vínculos?');">Excluir</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle CssClass="grid-pager" />
                <EmptyDataRowStyle CssClass="text-muted" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
