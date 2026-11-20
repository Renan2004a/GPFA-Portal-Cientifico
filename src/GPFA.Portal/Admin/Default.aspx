<%@ Page Title="Visão geral" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GPFA.Portal.Admin.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminTopbarActions" runat="server">
    <a class="btn-gpfa btn-brand btn-sm" href="<%= ResolveUrl("~/Admin/Projetos.aspx") %>">
        <svg class="icon icon-sm" aria-hidden="true"><use href="#i-plus"/></svg> Novo projeto
    </a>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Olá, <asp:Literal ID="litNomeUsuario" runat="server" />.</h2>
            <p>Este é o panorama do GPFA. Os indicadores são calculados diretamente do banco de dados.</p>
        </div>
        <span class="status-pill status-andamento">Sistema operacional</span>
    </div>

    <div class="kpi-grid">
        <div class="kpi">
            <div class="kpi-label">Pesquisadores</div>
            <div class="kpi-value"><asp:Literal ID="litPesquisadores" runat="server" /></div>
            <div class="kpi-note">perfis acadêmicos ativos</div>
        </div>
        <div class="kpi">
            <div class="kpi-label">Estudantes</div>
            <div class="kpi-value"><asp:Literal ID="litEstudantes" runat="server" /></div>
            <div class="kpi-note"><asp:Literal ID="litEstudantesAtivos" runat="server" /> em atividade</div>
        </div>
        <div class="kpi">
            <div class="kpi-label">Projetos</div>
            <div class="kpi-value"><asp:Literal ID="litProjetos" runat="server" /></div>
            <div class="kpi-note"><asp:Literal ID="litProjetosAndamento" runat="server" /> em andamento</div>
        </div>
        <div class="kpi">
            <div class="kpi-label">Verba captada</div>
            <div class="kpi-value" style="font-size:1.55rem"><asp:Literal ID="litVerba" runat="server" /></div>
            <div class="kpi-note"><asp:Literal ID="litFinanciamentos" runat="server" /> registros de financiamento</div>
        </div>
    </div>

    <div class="admin-grid">
        <div class="d-flex flex-column gap-3">
            <div class="panel-card">
                <div class="panel-card-head">
                    <h3>Projetos recentes</h3>
                    <a class="link-arrow" href="<%= ResolveUrl("~/Admin/Projetos.aspx") %>">Gerenciar</a>
                </div>
                <div class="data-list">
                    <asp:Repeater ID="rptProjetos" runat="server">
                        <ItemTemplate>
                            <div class="data-row">
                                <div class="data-row-main">
                                    <div>
                                        <p class="data-row-title">
                                            <a href='<%# ResolveUrl("~/Projeto.aspx") + "?id=" + Eval("ProjetoId") %>'><%#: Eval("Titulo") %></a>
                                        </p>
                                        <div class="data-row-sub"><%#: Eval("Sigla") %> · <%#: Eval("CoordenadorNome") %> · <%#: Eval("PeriodoFormatado") %></div>
                                    </div>
                                </div>
                                <div class="d-flex align-items-center gap-2">
                                    <span class='status-pill <%# Eval("SituacaoCss") %>'><%#: Eval("Situacao") %></span>
                                    <span class="data-row-value"><%#: Eval("TotalFormatado") %></span>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <asp:Panel ID="pnlSemProjetos" runat="server" Visible="false" CssClass="small muted">Nenhum projeto cadastrado.</asp:Panel>
            </div>

            <div class="panel-card">
                <div class="panel-card-head">
                    <h3>Produção científica</h3>
                    <a class="link-arrow" href="<%= ResolveUrl("~/Admin/Publicacoes.aspx") %>">Gerenciar</a>
                </div>
                <div class="bar-row">
                    <span>Publicações</span>
                    <div class="bar-track"><div id="divBarPublicacoes" runat="server" class="bar-fill"></div></div>
                    <span class="bar-value"><asp:Literal ID="litPublicacoes" runat="server" /></span>
                </div>
                <div class="bar-row">
                    <span>Destaques ativos</span>
                    <div class="bar-track"><div id="divBarDestaques" runat="server" class="bar-fill" style="background:linear-gradient(90deg,var(--accent),#f2c078)"></div></div>
                    <span class="bar-value"><asp:Literal ID="litComDestaque" runat="server" /></span>
                </div>
                <div class="bar-row">
                    <span>Projetos ativos</span>
                    <div class="bar-track"><div id="divBarProjetos" runat="server" class="bar-fill" style="background:linear-gradient(90deg,#4f9d69,var(--teal))"></div></div>
                    <span class="bar-value"><asp:Literal ID="litProjetosAndamentoBar" runat="server" /></span>
                </div>
                <p class="tiny muted mt-2 mb-0">Os destaques são limitados a cinco por pesquisador, conforme a regra RN05.</p>
            </div>

            <asp:PlaceHolder ID="phIntegridade" runat="server">
                <div class="panel-card">
                    <div class="panel-card-head">
                        <h3>Integridade dos dados</h3>
                        <a class="link-arrow" href="<%= ResolveUrl("~/Admin/Relatorios.aspx") %>">Relatórios</a>
                    </div>
                    <asp:Panel ID="pnlIntegridadeOk" runat="server" Visible="false" CssClass="alert-gpfa alert-success-gpfa mb-0">
                        Nenhuma inconsistência detectada: todos os estudantes ativos possuem orientador principal e projeto, e todas as publicações têm autores.
                    </asp:Panel>
                    <div class="data-list">
                        <asp:Repeater ID="rptAlertas" runat="server">
                            <ItemTemplate>
                                <div class="data-row">
                                    <div class="data-row-main">
                                        <svg class="icon" style="color:var(--warning);flex-shrink:0" aria-hidden="true"><use href="#i-alert"/></svg>
                                        <div>
                                            <p class="data-row-title"><%#: Eval("Alerta") %></p>
                                            <div class="data-row-sub"><%#: Eval("Detalhe") %> (registro #<%#: Eval("RegistroId") %>)</div>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <asp:Panel ID="pnlComAlertas" runat="server" Visible="false" CssClass="small muted mt-2">
                        Corrija os vínculos indicados para manter a consistência exigida pelas regras de negócio.
                    </asp:Panel>
                </div>
            </asp:PlaceHolder>
        </div>

        <div class="d-flex flex-column gap-3">
            <div class="panel-card">
                <div class="panel-card-head">
                    <h3>Estado do Web Forms</h3>
                </div>
                <p class="tiny muted">Demonstração do ciclo de vida e do gerenciamento de estado desta requisição.</p>
                <div class="panel-stat"><span>IsPostBack</span><span class="value" style="font-size:.9rem"><asp:Literal ID="litIsPostBack" runat="server" /></span></div>
                <div class="panel-stat"><span>Cliques no diagnóstico (ViewState)</span><span class="value" style="font-size:.9rem"><asp:Literal ID="litViewState" runat="server" /></span></div>
                <div class="panel-stat"><span>Eventos em Session</span><span class="value" style="font-size:.9rem"><asp:Literal ID="litSessaoEventos" runat="server" /></span></div>
                <div class="panel-stat"><span>Itens na Session</span><span class="value" style="font-size:.9rem"><asp:Literal ID="litSessaoItens" runat="server" /></span></div>
                <asp:Button ID="btnDiagnostico" runat="server" Text="Executar PostBack" CssClass="btn-gpfa btn-outline-brand btn-sm w-100 mt-2"
                    OnClick="btnDiagnostico_Click" CausesValidation="false" />
                <div class="mt-2"><asp:Literal ID="litEventos" runat="server" /></div>
            </div>

            <asp:PlaceHolder ID="phAuditoria" runat="server">
                <div class="panel-card">
                    <div class="panel-card-head">
                        <h3>Auditoria recente</h3>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="gvAuditoria" runat="server" AutoGenerateColumns="false" CssClass="table-gpfa"
                            GridLines="None" UseAccessibleHeader="true" EmptyDataText="Nenhuma ação registrada.">
                            <Columns>
                                <asp:BoundField DataField="DataHora" HeaderText="Quando" DataFormatString="{0:dd/MM HH:mm}" />
                                <asp:BoundField DataField="Acao" HeaderText="Ação" />
                                <asp:BoundField DataField="Entidade" HeaderText="Entidade" />
                            </Columns>
                            <EmptyDataRowStyle CssClass="text-muted small" />
                        </asp:GridView>
                    </div>
                </div>
            </asp:PlaceHolder>

            <div class="panel-card">
                <div class="panel-card-head"><h3>Ações rápidas</h3></div>
                <div class="d-grid gap-2">
                    <a class="btn-gpfa btn-outline-brand btn-sm text-start" href="<%= ResolveUrl("~/Admin/Financiamentos.aspx") %>">
                        <svg class="icon icon-sm" aria-hidden="true"><use href="#i-coins"/></svg> Registrar financiamento
                    </a>
                    <a class="btn-gpfa btn-outline-brand btn-sm text-start" href="<%= ResolveUrl("~/Admin/Estudantes.aspx") %>">
                        <svg class="icon icon-sm" aria-hidden="true"><use href="#i-graduation"/></svg> Cadastrar estudante
                    </a>
                    <a class="btn-gpfa btn-outline-brand btn-sm text-start" href="<%= ResolveUrl("~/Admin/Relatorios.aspx") %>">
                        <svg class="icon icon-sm" aria-hidden="true"><use href="#i-chart"/></svg> Abrir relatórios
                    </a>
                    <a class="btn-gpfa btn-outline-brand btn-sm text-start" href="<%= ResolveUrl("~/AlterarSenha.aspx") %>">
                        <svg class="icon icon-sm" aria-hidden="true"><use href="#i-lock"/></svg> Alterar minha senha
                    </a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
