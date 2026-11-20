<%@ Page Title="Participações" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Participacoes.aspx.cs" Inherits="GPFA.Portal.Admin.ParticipacoesAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Participações e vínculos</h2>
            <p>Gerencie os pesquisadores participantes e os estudantes vinculados a cada projeto. O coordenador é participante implícito.</p>
        </div>
    </div>

    <div class="filter-bar">
        <div class="filter-field" style="flex:3">
            <label for="<%= ddlProjeto.ClientID %>">Projeto</label>
            <asp:DropDownList ID="ddlProjeto" runat="server" CssClass="form-select" AutoPostBack="true"
                OnSelectedIndexChanged="ddlProjeto_SelectedIndexChanged" />
        </div>
        <div class="filter-actions">
            <a class="btn-gpfa btn-outline-brand" href="<%= ResolveUrl("~/Admin/Projetos.aspx") %>">Voltar aos projetos</a>
        </div>
    </div>

    <asp:Panel ID="pnlSemProjeto" runat="server" Visible="false" CssClass="empty-state">
        <div class="empty-icon"><svg class="icon icon-lg" aria-hidden="true"><use href="#i-flask"/></svg></div>
        <h3>Nenhum projeto disponível</h3>
        <p>Cadastre um projeto ou verifique suas permissões de coordenação.</p>
        <a class="btn-gpfa btn-brand" href="<%= ResolveUrl("~/Admin/Projetos.aspx") %>">Ir para projetos</a>
    </asp:Panel>

    <asp:Panel ID="pnlConteudo" runat="server" Visible="false">
        <div class="panel mb-4">
            <div class="d-flex justify-content-between align-items-start gap-3 flex-wrap">
                <div>
                    <div class="tiny muted text-uppercase" style="letter-spacing:.08em">Projeto selecionado</div>
                    <h3 style="font-size:1.35rem;margin:.2rem 0"><asp:Literal ID="litProjetoTitulo" runat="server" /></h3>
                    <div class="small muted"><asp:Literal ID="litProjetoInfo" runat="server" /></div>
                </div>
                <span class="status-pill status-concluido">Coordenação: <asp:Literal ID="litCoordenador" runat="server" /></span>
            </div>
        </div>

        <div class="admin-grid">
            <div class="d-flex flex-column gap-3">
                <div class="panel-card">
                    <div class="panel-card-head">
                        <h3>Pesquisadores participantes</h3>
                        <span class="tiny muted"><asp:Literal ID="litQtdParticipantes" runat="server" /></span>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gvParticipantes" runat="server" AutoGenerateColumns="false" DataKeyNames="ParticipacaoId"
                            CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                            OnRowCommand="gvParticipantes_RowCommand" EmptyDataText="Nenhum participante além da coordenação.">
                            <Columns>
                                <asp:BoundField DataField="PesquisadorNome" HeaderText="Pesquisador" />
                                <asp:BoundField DataField="Papel" HeaderText="Papel" />
                                <asp:BoundField DataField="DataEntrada" HeaderText="Entrada" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="DataSaida" HeaderText="Saída" DataFormatString="{0:dd/MM/yyyy}" NullDisplayText="—" />
                                <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEncerrar" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                            CommandName="Encerrar" CommandArgument='<%# Eval("ParticipacaoId") %>' CausesValidation="false"
                                            Visible='<%# Eval("DataSaida") == DBNull.Value %>'>Encerrar</asp:LinkButton>
                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                            CommandName="Excluir" CommandArgument='<%# Eval("ParticipacaoId") %>' CausesValidation="false"
                                            OnClientClick="return confirm('Remover este participante do projeto?');">Remover</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataRowStyle CssClass="text-muted" />
                        </asp:GridView>
                    </div>

                    <div class="divider"></div>
                    <div class="form-grid">
                        <div class="col-5">
                            <label class="form-label" for="<%= ddlPesquisador.ClientID %>">Pesquisador</label>
                            <asp:DropDownList ID="ddlPesquisador" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-4">
                            <label class="form-label" for="<%= txtPapel.ClientID %>">Papel</label>
                            <asp:TextBox ID="txtPapel" runat="server" CssClass="form-control" MaxLength="60" placeholder="Pesquisador colaborador" />
                        </div>
                        <div class="col-3">
                            <label class="form-label" for="<%= txtEntrada.ClientID %>">Entrada</label>
                            <asp:TextBox ID="txtEntrada" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                    </div>
                    <asp:Button ID="btnAdicionarParticipante" runat="server" Text="Adicionar participante"
                        CssClass="btn-gpfa btn-brand btn-sm mt-3 js-submit-once" OnClick="btnAdicionarParticipante_Click" CausesValidation="false" />
                </div>

                <div class="panel-card" id="estudantes">
                    <div class="panel-card-head">
                        <h3>Estudantes vinculados</h3>
                        <span class="tiny muted"><asp:Literal ID="litQtdEstudantes" runat="server" /></span>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gvEstudantes" runat="server" AutoGenerateColumns="false" DataKeyNames="EstudanteProjetoId"
                            CssClass="table-gpfa" GridLines="None" UseAccessibleHeader="true"
                            OnRowCommand="gvEstudantes_RowCommand" EmptyDataText="Nenhum estudante vinculado.">
                            <Columns>
                                <asp:BoundField DataField="EstudanteNome" HeaderText="Estudante" />
                                <asp:BoundField DataField="TipoVinculo" HeaderText="Vínculo" NullDisplayText="—" />
                                <asp:BoundField DataField="DataEntrada" HeaderText="Entrada" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="DataSaida" HeaderText="Saída" DataFormatString="{0:dd/MM/yyyy}" NullDisplayText="—" />
                                <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="actions">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEncerrarV" runat="server" CssClass="btn-gpfa btn-outline-brand btn-sm"
                                            CommandName="Encerrar" CommandArgument='<%# Eval("EstudanteProjetoId") %>' CausesValidation="false"
                                            Visible='<%# Eval("DataSaida") == DBNull.Value %>'>Encerrar</asp:LinkButton>
                                        <asp:LinkButton ID="lnkExcluirV" runat="server" CssClass="btn-gpfa btn-danger-soft btn-sm"
                                            CommandName="Excluir" CommandArgument='<%# Eval("EstudanteProjetoId") %>' CausesValidation="false"
                                            OnClientClick="return confirm('Excluir este vínculo? O sistema bloqueia se for o último projeto ativo do estudante.');">Excluir</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataRowStyle CssClass="text-muted" />
                        </asp:GridView>
                    </div>

                    <div class="divider"></div>
                    <div class="form-grid">
                        <div class="col-5">
                            <label class="form-label" for="<%= ddlEstudante.ClientID %>">Estudante ativo</label>
                            <asp:DropDownList ID="ddlEstudante" runat="server" CssClass="form-select" />
                        </div>
                        <div class="col-4">
                            <label class="form-label" for="<%= txtTipoVinculo.ClientID %>">Tipo de vínculo</label>
                            <asp:TextBox ID="txtTipoVinculo" runat="server" CssClass="form-control" MaxLength="60" placeholder="Bolsa de iniciação científica" />
                        </div>
                        <div class="col-3">
                            <label class="form-label" for="<%= txtEntradaEstudante.ClientID %>">Entrada</label>
                            <asp:TextBox ID="txtEntradaEstudante" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                    </div>
                    <asp:Button ID="btnVincularEstudante" runat="server" Text="Vincular estudante"
                        CssClass="btn-gpfa btn-brand btn-sm mt-3 js-submit-once" OnClick="btnVincularEstudante_Click" CausesValidation="false" />
                </div>
            </div>

            <div class="d-flex flex-column gap-3">
                <div class="panel-card">
                    <div class="panel-card-head"><h3>Encerrar vínculo</h3></div>
                    <asp:HiddenField ID="hdnTipo" runat="server" Value="" />
                    <asp:HiddenField ID="hdnVinculoId" runat="server" Value="0" />
                    <p class="tiny muted"><asp:Literal ID="litEncerrarInfo" runat="server" /></p>
                    <label class="form-label" for="<%= txtDataSaida.ClientID %>">Data de saída</label>
                    <asp:TextBox ID="txtDataSaida" runat="server" CssClass="form-control" TextMode="Date" />
                    <asp:Button ID="btnEncerrarVinculo" runat="server" Text="Confirmar encerramento"
                        CssClass="btn-gpfa btn-brand btn-sm w-100 mt-2 js-submit-once" OnClick="btnEncerrarVinculo_Click" CausesValidation="false" />
                    <p class="tiny muted mt-2 mb-0">O último projeto ativo de um estudante ativo não pode ser encerrado (RN01).</p>
                </div>
                <div class="panel-card">
                    <div class="panel-card-head"><h3>Regras aplicadas</h3></div>
                    <ul class="small muted" style="padding-left:1.1rem;margin:0">
                        <li>Coordenador não é duplicado como participante (RN03).</li>
                        <li>Estudantes ativos mantêm ao menos um projeto (RN01).</li>
                        <li>Datas de saída não podem ser futuras.</li>
                        <li>Exclusões com vínculos são bloqueadas por FKs e triggers.</li>
                    </ul>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
