<%@ Control Language="C#" AutoEventWireup="true" Inherits="LitiumStudio_ECommerce_Panels_OrderTotal" Codebehind="OrderTotal.ascx.cs" %>
<%--<asp:ScriptManager runat="server" ID="c_myScriptManager" />--%>
<div style="margin: 15px 15px 15px 15px; width: 615px;">
    <asp:UpdatePanel runat="server" ID="c_mainUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <Foundation:InformationGroup runat="server" HeaderTextName="Settings">
                <table border="0" cellpadding="0" cellspacing="0" style="margin-top: 10px;">
                    <tr>
                        <td style="width: 300px; vertical-align: top;">
                            <div class="litText"><Foundation:ModuleString ID="ModuleString7" runat="server" Name="Currency" /></div>
                            <asp:DropDownList runat="server" ID="c_currency" AutoPostBack="true" OnSelectedIndexChanged="ChangeCurrency_Click" CssClass="litText" Width="160px"/>
                        </td>
                        <td style="width: 15px;">
                            &nbsp;

                        </td>
                        <td style="width: 300px; vertical-align: top;">
                         <div class="litText"><Foundation:ModuleString ID="ModuleString13" runat="server" Name="FilterWebSite" /></div>
                            <asp:DropDownList runat="server" ID="c_webSite" AutoPostBack="true" OnSelectedIndexChanged="ChangeWebSite_Click" CssClass="litText" Width="160px"/>  
                            
                        </td>
                        <td style="width: 15px;">
                            &nbsp;

                        </td>
                        <td style="width: 300px; vertical-align: top;"> 
                            <div class="litText"><Foundation:ModuleString ID="ModuleString8" runat="server" Name="Campaigns" /></div>
                            <asp:DropDownList runat="server" ID="c_campaign" AutoPostBack="true" DataTextField="Name"
                                DataValueField="ID" OnSelectedIndexChanged="Update_Click" CssClass="litText" Width="160px"/>
                        </td>
                    </tr>
                </table>
            </Foundation:InformationGroup>
            <br />
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td style="width: 300px; vertical-align: top;">
                        <asp:UpdatePanel runat="server" ID="c_linkUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <Foundation:InformationGroup ID="InformationGroup1" Height="210px" runat="server"
                                    HeaderText="Quick links" HeaderTextName="ReportQuickLink">
                                        <ul class="LeftLink" style="list-style-type: none; margin-left: 0px; padding-left: 0px; margin-top: 0px; padding-top: 0px;">
                                            <li>
                                                <asp:LinkButton ID="LinkButton1" CommandArgument="Today" runat="server" CssClass="SelectedLeftLinkReport" OnClick="QuickLink_Click">
                                                    <Foundation:ModuleString ID="ModuleString1" runat="server" Name="ReportQuickLinkToday" />
                                                </asp:LinkButton></li>
                                            <li>
                                                <asp:LinkButton ID="LinkButton2" CommandArgument="Yesterday" runat="server" CssClass="SelectedLeftLinkReport" OnClick="QuickLink_Click">
                                                    <Foundation:ModuleString ID="ModuleString2" runat="server" Name="ReportQuickLinkYesterday" />
                                                </asp:LinkButton></li>
                                            <li>
                                                <asp:LinkButton ID="LinkButton3" CommandArgument="LastWeek" runat="server" CssClass="SelectedLeftLinkReport" OnClick="QuickLink_Click">
                                                    <Foundation:ModuleString ID="ModuleString3" runat="server" Name="ReportQuickLinkLastWeek" />
                                                </asp:LinkButton></li>
                                            <li>
                                                <asp:LinkButton ID="LinkButton4" CommandArgument="LastMonth" runat="server" CssClass="SelectedLeftLinkReport" OnClick="QuickLink_Click">
                                                    <Foundation:ModuleString ID="ModuleString4" runat="server" Name="ReportQuickLinkLastMonth" />
                                                </asp:LinkButton></li>
                                            <li>
                                                <asp:LinkButton ID="LinkButton5" CommandArgument="LastSixMonths" runat="server" CssClass="SelectedLeftLinkReport" OnClick="QuickLink_Click">
                                                    <Foundation:ModuleString ID="ModuleString5" runat="server" Name="ReportQuickLinkLastSixMonths" />
                                                </asp:LinkButton></li>
                                            <li>
                                                <asp:LinkButton ID="LinkButton6" CommandArgument="LastYear" runat="server" CssClass="SelectedLeftLinkReport" OnClick="QuickLink_Click">
                                                    <Foundation:ModuleString ID="ModuleString6" runat="server" Name="ReportQuickLinkLastYear" />
                                                </asp:LinkButton></li>
                                        </ul>
                                </Foundation:InformationGroup>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 15px;">
                        &nbsp;
                    </td>
                    <td style="width: 300px; vertical-align: top;">
                        <asp:UpdatePanel runat="server" ID="c_calenderUpdatePanel" ChildrenAsTriggers="false"
                            UpdateMode="Conditional">
                            <ContentTemplate>
                                <Foundation:InformationGroup ID="InformationGroup2" Height="210px" runat="server"
                                    HeaderText="Custom interval" HeaderTextName="ReportCustomInterval">
                                    <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td width="45%">
                                                <div class="litInputTable">
                                                    <LS:DateTimeHelper ID="DateTimeHelper1" runat="server" AssociatedControlID="c_calendarFromDate" />
                                                    <Telerik:RadDatePicker runat="server" ID="c_calendarFromDate" Width="135" AutoPostBack="false" EnableTyping="false">
	                                                    <ClientEvents OnDateSelected="DateChanged"></ClientEvents>
                                                    </Telerik:RadDatePicker></div>

                                            </td>
                                            <td>
                                                -
                                            </td>
                                            <td width="45%">
                                                <div class="litInputTable">
                                                    <LS:DateTimeHelper ID="DateTimeHelper2" runat="server" AssociatedControlID="c_calendarToDate" />
                                                    <Telerik:RadDatePicker runat="server" ID="c_calendarToDate" Width="135" AutoPostBack="false" EnableTyping="false" >
	                                                    <ClientEvents OnDateSelected="DateChanged"></ClientEvents>
                                                    </Telerik:RadDatePicker></div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" style="height: 10px;"><asp:CompareValidator ID="c_dateCompareValidator" runat="server" ControlToValidate="c_calendarToDate"
			                                ControlToCompare="c_calendarFromDate" Operator="GreaterThanEqual" Type="Date">
		                                </asp:CompareValidator></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <span class="litText"><Foundation:ModuleString ID="ModuleString9" runat="server" Name="ReportInterval"/>:</span>
                                                <asp:DropDownList runat="server" ID="c_dateInterval" AutoPostBack="true" CssClass="litText" />
                                            </td>
                                            <td>
                                                <asp:Button runat="server"  CssClass="litInputButton" ID="c_update" OnClick="Update_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </Foundation:InformationGroup>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
	<div style="margin-top: 15px; margin-bottom: 10px;">
    <Foundation:InformationGroup ID="InformationGroup3" runat="server" HeaderTextName="ReportDiagramBoxHeadline">
        <asp:UpdatePanel runat="server" ID="c_diagramPanel" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <div>
					<table border="0" cellpadding="0" cellspacing="0" width="100%">
						<tr>
							<td style="width:50%;">
								<span class="litText"><Foundation:ModuleString ID="ModuleString10" runat="server" Name="ReportDiagramOrderValue" /> <asp:Label runat="server" ID="c_orderTotalValue" CssClass="litText" />
								<br />
								<Foundation:ModuleString ID="ModuleString11" runat="server" Name="ReportDiagramOrderCount" /> <asp:Literal runat="server" ID="c_orderCount"></asp:Literal></span>							
							</td>
							<td style="vertical-align:top; padding-right:13px;" align="right">
								<span class="litText"><Foundation:ModuleString ID="ModuleString12" runat="server" Name="OrderStatus" />:</span>
			                    <asp:DropDownList runat="server" ID="c_orderStatus" AutoPostBack="true" OnSelectedIndexChanged="Update_Click" CssClass="litText" />							
							</td>
						</tr>
					</table>
                </div>
                <ComponentArtChart:Chart ID="c_chartVisits" runat="server" Height="400px" Width="580" style="margin-right: 10px;" SaveImageOnDisk="false" CacheInterval="10" DeletionDelay="10"/>
            </ContentTemplate>
        </asp:UpdatePanel>
    </Foundation:InformationGroup>
	</div>
</div>
<script>
	var fromDateId = "<%=c_calendarFromDate.ClientID%>";
	var toDateId = "<%=c_calendarToDate.ClientID%>";
	var intervalId = "<%=c_dateInterval.ClientID%>";

	function daydiff(first, second) {
		return (second - first) / (1000 * 60 * 60 * 24);
	}

	function DateChanged(sender, args) {
		var days = daydiff($find(fromDateId).get_dateInput()._value, $find(toDateId).get_dateInput()._value);
		$("#" + intervalId)[0][4].disabled = days > 0;
		$("#" + intervalId)[0][3].disabled = days > 31;
		$("#" + intervalId)[0][2].disabled = days > 100;

		if ($("#" + intervalId)[0][4].selected && $("#" + intervalId)[0][4].disabled) {
			$("#" + intervalId)[0][4].selected = false;
			$("#" + intervalId)[0][3].selected = true;
		}

		if ($("#" + intervalId)[0][3].selected && $("#" + intervalId)[0][3].disabled) {
			$("#" + intervalId)[0][3].selected = false;
			$("#" + intervalId)[0][2].selected = true;
		}

		if ($("#" + intervalId)[0][2].selected && $("#" + intervalId)[0][2].disabled) {
			$("#" + intervalId)[0][2].selected = false;
			$("#" + intervalId)[0][1].selected = true;
		}
	}
</script>