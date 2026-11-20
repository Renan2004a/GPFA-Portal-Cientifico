<%@ Page Title="Relatórios" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Relatorios.aspx.cs" Inherits="GPFA.Portal.Admin.RelatoriosAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AdminTopbarActions" runat="server">
    <button type="button" class="btn-gpfa btn-outline-brand btn-sm" onclick="window.print()">
        <svg class="icon icon-sm" aria-hidden="true"><use href="#i-file"/></svg> Imprimir
    </button>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="AdminContent" runat="server">
    <div class="admin-page-head">
        <div>
            <h2>Relatórios administrativos</h2>
            <p>Indicadores calculados diretamente das views do banco. Valores financeiros são somados por projeto, sem duplicidade.</p>
        </div>
        <span class="tiny muted">Atualizado em <asp:Literal ID="litAtualizado" runat="server" /></span>
    </div>

    <div class="kpi-grid">
        <div class="kpi">
            <div class="kpi-label">Total de projetos</div>
            <div class="kpi-value"><asp:Literal ID="litProjetos" runat="server" /></div>
            <div class="kpi-note"><asp:Literal ID="litProjetosAndamento" runat="server" /> em andamento</div>
        </div>
        <div class="kpi">
            <div class="kpi-label">Pesquisadores</div>
            <div class="kpi-value"><asp:Literal ID="litPesquisadores" runat="server" /></div>
        </div>
        <div class="kpi">
            <div class="kpi-label">Estudantes</div>
            <div class="kpi-value"><asp:Literal ID="litEstudantes" runat="server" /></div>
            <div class="kpi-note"><asp:Literal ID="litEstudantesAtivos" runat="server" /> ativos</div>
        </div>
        <div class="kpi">
            <div class="kpi-label">Recursos captados</div>
            <div class="kpi-value" style="font-size:1.55rem"><asp:Literal ID="litTotalCaptado" runat="server" /></div>
            <div class="kpi-note"><asp:Literal ID="litQtdFinanciamentos" runat="server" /> registros</div>
        </div>
    </div>

    <div class="admin-grid">
        <div class="panel-card">
            <div class="panel-card-head"><h3>Projetos por situação</h3></div>
            <asp:Repeater ID="rptSituacao" runat="server">
                <ItemTemplate>
                    <div class="bar-row">
                        <span><%#: Eval("Rotulo") %></span>
                        <div class="bar-track"><div class="bar-fill" style='width:<%# Eval("Percentual", "{0:0.#}") %>%'></div></div>
                        <span class="bar-value"><%#: Eval("Quantidade") %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="panel-card">
            <div class="panel-card-head"><h3>Publicações por ano</h3></div>
            <asp:Repeater ID="rptPublicacoesAno" runat="server">
                <ItemTemplate>
                    <div class="bar-row" style="grid-template-columns:60px 1fr 40px">
                        <span><%#: Eval("Rotulo") %></span>
                        <div class="bar-track"><div class="bar-fill" style="width:<%# CalcularLarguraAno(Eval("Quantidade")) %>%"></div></div>
                        <span class="bar-value"><%#: Eval("Quantidade") %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <div class="admin-grid mt-3">
        <div class="panel-card">
            <div class="panel-card-head">
                <h3>Financiamentos por projeto</h3>
                <span class="tiny muted">Total: <asp:Literal ID="litTotalProjetos" runat="server" /></span>
            </div>
            <div class="table-responsive">
                <table class="table-gpfa">
                    <thead><tr><th>Projeto</th><th style="text-align:center">Registros</th><th style="text-align:right">Valor</th></tr></thead>
                    <tbody>
                        <asp:Repeater ID="rptFinProjeto" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%#: Eval("Rotulo") %></td>
                                    <td class="text-center"><%#: Eval("Quantidade") %></td>
                                    <td style="text-align:right;font-weight:600"><%#: Eval("ValorFormatado") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="panel-card">
            <div class="panel-card-head"><h3>Financiamentos por fonte</h3></div>
            <asp:Repeater ID="rptFinFonte" runat="server">
                <ItemTemplate>
                    <div class="bar-row" style="grid-template-columns:100px 1fr 80px">
                        <span><%#: Eval("Rotulo") %></span>
                        <div class="bar-track"><div class="bar-fill" style='width:<%# Eval("Percentual", "{0:0.#}") %>%'></div></div>
                        <span class="bar-value" style="font-size:.7rem"><%#: Eval("ValorFormatado") %></span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div class="divider"></div>
            <div class="panel-stat"><span>Total por fonte</span><span class="value" style="font-size:1.1rem"><asp:Literal ID="litTotalFontes" runat="server" /></span></div>
        </div>
    </div>

    <div class="admin-grid mt-3">
        <div class="panel-card">
            <div class="panel-card-head"><h3>Publicações por pesquisador</h3></div>
            <div class="table-responsive">
                <table class="table-gpfa">
                    <thead><tr><th>Pesquisador</th><th style="text-align:center">Publicações</th></tr></thead>
                    <tbody>
                        <asp:Repeater ID="rptPubPesquisador" runat="server">
                            <ItemTemplate>
                                <tr><td><%#: Eval("Rotulo") %></td><td class="text-center"><%#: Eval("Quantidade") %></td></tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>

        <div class="panel-card">
            <div class="panel-card-head"><h3>Orientandos atuais e passados</h3></div>
            <div class="table-responsive">
                <table class="table-gpfa">
                    <thead><tr><th>Orientador</th><th style="text-align:center">Atuais</th><th style="text-align:center">Passados</th><th style="text-align:center">Total</th></tr></thead>
                    <tbody>
                        <asp:Repeater ID="rptOrientadores" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td><%#: Eval("OrientadorNome") %></td>
                                    <td class="text-center"><%#: Eval("Atuais") %></td>
                                    <td class="text-center"><%#: Eval("Passados") %></td>
                                    <td class="text-center"><strong><%#: Eval("Total") %></strong></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <div class="panel-card mt-3">
        <div class="panel-card-head">
            <h3>Total captado por pesquisador</h3>
            <span class="tiny muted">Projetos compartilhados são contados uma vez por pesquisador, não uma vez por participante</span>
        </div>
        <div class="table-responsive">
            <table class="table-gpfa">
                <thead><tr><th>Pesquisador</th><th>Projetos vinculados</th><th style="text-align:right">Total captado</th></tr></thead>
                <tbody>
                    <asp:Repeater ID="rptPesquisadorTotal" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%#: Eval("Rotulo") %></td>
                                <td class="text-center"><%#: Eval("Quantidade") %></td>
                                <td style="text-align:right;font-weight:600"><%#: Eval("ValorFormatado") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <p class="tiny muted mt-2 mb-0">A soma dos totais por pesquisador não é igual ao total geral do grupo, pois um mesmo financiamento pode apoiar vários pesquisadores. O total geral considera cada registro de financiamento uma única vez.</p>
    </div>

    <asp:Panel ID="pnlIntegridade" runat="server" CssClass="panel-card mt-3">
        <div class="panel-card-head">
            <h3>Alertas de integridade</h3>
            <span class="status-pill status-andamento">Monitoramento contínuo</span>
        </div>
        <asp:Panel ID="pnlSemAlertas" runat="server" Visible="false" CssClass="small" style="color:var(--success)">
            Nenhuma inconsistência encontrada nas regras de vínculo, orientação e autoria.
        </asp:Panel>
        <div class="table-responsive">
            <table class="table-gpfa">
                <thead><tr><th>Alerta</th><th>Registro</th><th>Detalhe</th></tr></thead>
                <tbody>
                    <asp:Repeater ID="rptAlertas" runat="server">
                        <ItemTemplate>
                            <tr><td><%#: Eval("Alerta") %></td><td>#<%#: Eval("RegistroId") %></td><td><%#: Eval("Detalhe") %></td></tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
