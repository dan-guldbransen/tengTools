using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Litium.Studio.UI.ECommerce.Common.Constants;
using ComponentArt.Web.Visualization.Charting;
using System.Drawing;
using Litium.Foundation.Modules.ECommerce;
using Litium.Foundation.Modules.ECommerce.Statistics;
using System.Globalization;
using System.Linq;
using Litium;
using Litium.Foundation.Extenssions;
using Litium.Foundation.Modules.CMS;
using Litium.Globalization;
using Litium.Websites;

public partial class LitiumStudio_ECommerce_Panels_OrderTotal : Litium.Studio.UI.ECommerce.Common.ReportBaseUserControl
{
    private readonly ChannelService _channelService;
    private readonly CountryService _countryService;
    private readonly WebsiteService _websiteService;
    public LitiumStudio_ECommerce_Panels_OrderTotal(WebsiteService websiteService, ChannelService channelService, CountryService countryService)
    {
        _websiteService = websiteService;
        _channelService = channelService;
        _countryService = countryService;
    }

    /// <summary>
    /// Page load.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        if (!Page.IsPostBack)
        {
            InitControls();
            QuickLink_Click(LinkButton1, e);
        }
    }

    /// <summary>
    /// Handles the Click event of the QuickLink control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void QuickLink_Click(object sender, EventArgs e)
    {
        RemoveLastLink();

        c_linkUpdatePanel.Update();
        LinkButton quickLink = sender as LinkButton;
        if (quickLink != null)
        {
            LastLinkID = quickLink.ID;
            quickLink.CssClass = "SelectedLeftLinkReport";
            c_dateInterval.SelectedIndex = -1;
			if (quickLink.CommandArgument.Equals("Today"))
            {
                c_calendarFromDate.SelectedDate = DateTime.Now;
                c_calendarToDate.SelectedDate = DateTime.Now;
                c_dateInterval.Items.FindByValue(StatisticInterval.Hour.ToString()).Selected = true;
			}
            else if (quickLink.CommandArgument.Equals("Yesterday"))
            {
                c_calendarFromDate.SelectedDate = DateTime.Now.AddDays(-1);
                c_calendarToDate.SelectedDate = DateTime.Now.AddDays(-1);
                c_dateInterval.Items.FindByValue(StatisticInterval.Hour.ToString()).Selected = true;
			}
            else if (quickLink.CommandArgument.Equals("LastWeek"))
            {
                c_calendarFromDate.SelectedDate = DateTime.Now.AddDays(-6);
                c_calendarToDate.SelectedDate = DateTime.Now;
                c_dateInterval.Items.FindByValue(StatisticInterval.Day.ToString()).Selected = true;
			}
            else if (quickLink.CommandArgument.Equals("LastMonth"))
            {
                c_calendarFromDate.SelectedDate = DateTime.Now.AddMonths(-1);
                c_calendarToDate.SelectedDate = DateTime.Now;
                c_dateInterval.Items.FindByValue(StatisticInterval.Week.ToString()).Selected = true;
			}
            else if (quickLink.CommandArgument.Equals("LastSixMonths"))
            {
                c_calendarFromDate.SelectedDate = DateTime.Now.AddMonths(-6);
                c_calendarToDate.SelectedDate = DateTime.Now;
				c_dateInterval.Items.FindByValue(StatisticInterval.Month.ToString()).Selected = true;
			}
            else if (quickLink.CommandArgument.Equals("LastYear"))
            {
                c_calendarFromDate.SelectedDate = DateTime.Now.AddYears(-1);
                c_calendarToDate.SelectedDate = DateTime.Now;
                c_dateInterval.Items.FindByValue(StatisticInterval.Month.ToString()).Selected = true;
			}

			c_dateInterval.Items.FindByValue(StatisticInterval.Hour.ToString()).Attributes.Remove("disabled");
			c_dateInterval.Items.FindByValue(StatisticInterval.Day.ToString()).Attributes.Remove("disabled");
			c_dateInterval.Items.FindByValue(StatisticInterval.Week.ToString()).Attributes.Remove("disabled");

			var d = (c_calendarToDate.SelectedDate.Value - c_calendarFromDate.SelectedDate.Value);
			if (d.TotalDays > 0)
				c_dateInterval.Items.FindByValue(StatisticInterval.Hour.ToString()).Attributes["disabled"] = "disabled";
			if (d.TotalDays > 31)
				c_dateInterval.Items.FindByValue(StatisticInterval.Day.ToString()).Attributes["disabled"] = "disabled";
			if (d.TotalDays > 100)
				c_dateInterval.Items.FindByValue(StatisticInterval.Week.ToString()).Attributes["disabled"] = "disabled";

            c_calenderUpdatePanel.Update();
        }
        Update_Click(sender, e);
    }

    /// <summary>
    /// Handles the Click event of the Update control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Update_Click(object sender, EventArgs e)
    {
        if (sender is Button)
            RemoveLastLink();

        Currency currency = IoC.Resolve<CurrencyService>().Get(new Guid(c_currency.SelectedValue));
        
        // Update statistic.
        string[] labels;
        decimal[] values;
        GetChartData(currency, new Guid(c_webSite.SelectedValue), new Guid(c_campaign.SelectedValue), Convert.ToInt16(c_orderStatus.SelectedValue), (StatisticInterval)Enum.Parse(typeof(StatisticInterval), c_dateInterval.SelectedValue), c_calendarFromDate.SelectedDate.Value, c_calendarToDate.SelectedDate.Value, out labels, out values);
        CreateChart(currency, labels, values);
        c_diagramPanel.Update();
    }

    /// <summary>
    /// Handles the Click event of the change currency control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ChangeCurrency_Click(object sender, EventArgs e)
    {
        var currencyID = new Guid(c_currency.SelectedValue);
        InitWebSiteDropDown(currencyID);
        InitCampaignDropDown(currencyID, Guid.Empty);
        Update_Click(sender, e);
        c_mainUpdatePanel.Update();
    }

    /// <summary>
    /// Handles the Click event of the change website control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ChangeWebSite_Click(object sender, EventArgs e)
    {
        InitCampaignDropDown(new Guid(c_currency.SelectedValue), Guid.Empty);
        Update_Click(sender, e);
        c_mainUpdatePanel.Update();

    }

    /// <summary>
    /// Inits the controls.
    /// </summary>
    private void InitControls()
    {
        // add intervals
        c_dateInterval.Items.Add(new ListItem(CurrentModule.Strings.GetValue("ReportYears", FoundationContext.LanguageID, true), StatisticInterval.Year.ToString()));
        c_dateInterval.Items.Add(new ListItem(CurrentModule.Strings.GetValue("ReportMonths", FoundationContext.LanguageID, true), StatisticInterval.Month.ToString()));
        c_dateInterval.Items.Add(new ListItem(CurrentModule.Strings.GetValue("ReportWeeks", FoundationContext.LanguageID, true), StatisticInterval.Week.ToString()));
        c_dateInterval.Items.Add(new ListItem(CurrentModule.Strings.GetValue("ReportDays", FoundationContext.LanguageID, true), StatisticInterval.Day.ToString()));
        c_dateInterval.Items.Add(new ListItem(CurrentModule.Strings.GetValue("ReportHours", FoundationContext.LanguageID, true), StatisticInterval.Hour.ToString()));

        c_dateCompareValidator.ErrorMessage = CurrentModule.Strings.GetValue("InvalidDateSelection", FoundationContext.LanguageID, true);

        // add currencies
        List<Currency> currencies = IoC.Resolve<CurrencyService>().GetAll().ToList();
        currencies.Sort((a, b) => string.Compare(a.GetCurrencyEnglishName(), b.GetCurrencyEnglishName(), StringComparison.Ordinal));
        currencies.ForEach(delegate(Currency currency) { c_currency.Items.Add(new ListItem(currency.GetCurrencyEnglishName(), currency.SystemId.ToString())); });

        // Add campaigns
        InitCampaignDropDown(currencies[0].SystemId, Guid.Empty);

        // Add Order Statuses
		Dictionary<short, string> states = ModuleECommerce.Instance.Orders.GetAllOrderStates(FoundationContext.Token);
    	foreach (KeyValuePair<short, string> state in states)
    	{
    		c_orderStatus.Items.Add(new ListItem(state.Value, state.Key.ToString()));
    	}
		c_orderStatus.Items.Insert(0, new ListItem(CurrentModule.Strings.GetValue("AllStatus", FoundationContext.LanguageID, true), "-1"));

        InitWebSiteDropDown(currencies[0].SystemId);

        // set text
        c_update.Text = CurrentModule.Strings.GetValue("Update", FoundationContext.LanguageID, true);
    }

    /// <summary>
    /// Init campaign drop down
    /// </summary>
    /// <param name="currencyID">CurrencyID</param>
    private void InitWebSiteDropDown(Guid currencyID)
    {
        // Add web sites
        c_webSite.Items.Clear();
        if (ModuleCMS.ExistsInstance)
        {
            var countries = _countryService.GetAll().Where(x => x.CurrencySystemId == currencyID).Select(c => c.SystemId).ToList();
            var websiteIds = _channelService.GetAll().Where(c => c.WebsiteSystemId != null && c.CountryLinks.Any(x => countries.Contains(x.CountrySystemId))).Select(y => y.WebsiteSystemId.GetValueOrDefault()).Distinct();
            foreach (Website webSite in websiteIds.Select(x=>_websiteService.Get(x)))
            {
                ListItem item = new ListItem(webSite.Localizations.CurrentUICulture.Name, webSite.SystemId.ToString());
                c_webSite.Items.Add(item);
            }
        }
        ListItem itemAll = new ListItem(CurrentModule.Strings.GetValue(ModuleStringConstants.ALL_WEB_SITES, FoundationContext.LanguageID, true), Guid.Empty.ToString());
        c_webSite.Items.Insert(0, itemAll);
        c_webSite.SelectedIndex = -1;
        c_webSite.DataBind();
    }

    /// <summary>
    /// Init campaign drop down
    /// </summary>
    /// <param name="currencyID">CurrencyID</param>
    /// <param name="channelId">ChannelId, if Guid.Empty show campaign for all channel.</param>
    private void InitCampaignDropDown(Guid currencyID, Guid channelId)
    {
        var campaigns = CurrentModule.Campaigns.GetAll();
        campaigns.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
        c_campaign.DataSource = campaigns.FindAll(item => item.Data.Currencies.Any(x => x == currencyID) && (channelId == Guid.Empty || item.Data.Channels.Any(x => x.ChannelId == channelId)));
        c_campaign.DataBind();
        c_campaign.Items.Insert(0, new ListItem(CurrentModule.Strings.GetValue("ReportChoiceCampaign", FoundationContext.LanguageID, true), Guid.Empty.ToString()));
    }

    /// <summary>
    /// Gets the chart data.
    /// </summary>
    /// <param name="currency">The currency.</param>
    /// <param name="webSiteID">The webSite ID.</param>
    /// <param name="campaignID">The campaign ID.</param>
    /// <param name="orderStatus">The order status</param>
    /// <param name="graphInterval">The graph interval.</param>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="labels">The labels.</param>
    /// <param name="values">The values.</param>
    private void GetChartData(Currency currency, Guid webSiteID, Guid campaignID, short orderStatus, StatisticInterval graphInterval, DateTime startDate, DateTime endDate, out string[] labels, out decimal[] values)
    {
        try
        {
            long orderCount;
            decimal orderValue;

            if (startDate < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                startDate = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            if (endDate > (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                endDate = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            if (startDate > endDate)
                throw new Exception(ModuleECommerce.Instance.Strings.GetValue("ECommerceWrongDate", FoundationContext.LanguageID, true));
            List<Litium.Foundation.Modules.ECommerce.Carriers.StatisticCarrier> result = CurrentModule.Statistics.GetTotalOrderStatistic(webSiteID, currency.SystemId, campaignID, orderStatus, graphInterval, startDate, endDate, out orderCount, out orderValue);

            var numberFormatInfo = currency.GetNumberFormatInfo(CultureInfo.CurrentCulture);
            c_orderTotalValue.Text = orderValue.ToString(numberFormatInfo);
            c_orderCount.Text = orderCount.ToString(numberFormatInfo);

            labels = result.ConvertAll<string>(delegate(Litium.Foundation.Modules.ECommerce.Carriers.StatisticCarrier carrier) { return carrier.Name; }).ToArray();
            values = result.ConvertAll<decimal>(delegate(Litium.Foundation.Modules.ECommerce.Carriers.StatisticCarrier carrier) { return carrier.Value; }).ToArray();
        }
        catch (Exception e)
        {
            labels = new string[0];
            values = new decimal[0];
            Litium.Studio.UI.Common.InformationPanel.DisplayErrorMessage(e.Message);
        }
        
    }

    /// <summary>
    /// Creates the chart.
    /// </summary>
    /// <param name="currency">The currency.</param>
    /// <param name="labels">The labels.</param>
    /// <param name="values">The values.</param>
    private void CreateChart(Currency currency, string[] labels, decimal[] values)
    {
        // Label style (x and y label)
        LabelStyle labelStyleX = new LabelStyle();
        labelStyleX.Font = new Font("Arial", 10);
        labelStyleX.ForeColor = Color.Black;
        labelStyleX.Name = "xStyle";
        c_chartVisits.LabelStyles.Add(labelStyleX);

        // Data point style
        DataPointLabelStyle dataPointStyle = c_chartVisits.DataPointLabelStyles.CreateNew("CustomStyle");
        if (dataPointStyle != null)
        {
            dataPointStyle.ForeColor = System.Drawing.Color.Black;
            dataPointStyle.Font = new System.Drawing.Font("Arial", 10, System.Drawing.GraphicsUnit.Point);
            dataPointStyle.LabelPosition = LabelPositionKind.AboveVertical;
        }

        // Chart settings
        c_chartVisits.View.Kind = ProjectionKind.TwoDimensional;
        c_chartVisits.View.LightsOffOn2D = false;
        c_chartVisits.ResizeMarginsToFitLabels = true;
        //c_chartVisits.MainStyle = "Line";

        if (ViewState["ChartWidth"] != null)
            c_chartVisits.Width = new Unit((double)ViewState["ChartWidth"]);
        else
            ViewState["ChartWidth"] = c_chartVisits.Width.Value;


        // If more than 24 columns, 
        if (labels.Length > 24)
        {
            c_chartVisits.Width = labels.Length * 23;
            //title.Font = new System.Drawing.Font("Arial", 10, System.Drawing.GraphicsUnit.Point);
            dataPointStyle.Font = new System.Drawing.Font("Arial", 10, System.Drawing.GraphicsUnit.Point);
        }

        // Line style
        LineStyle2D lineStyle2D = new LineStyle2D();
        lineStyle2D.Color = ColorTranslator.FromHtml("#0266fb");
        lineStyle2D.Width = 2;
        lineStyle2D.ShadeWidth = 2;
        c_chartVisits.LineStyles2D.Add(lineStyle2D);

        // Series style
        SeriesStyle seriesStyle = new SeriesStyle();
        seriesStyle.ChartKind = ChartKind.Rectangle;
        seriesStyle.LineStyle2DName = lineStyle2D.Name;
        c_chartVisits.SeriesStyles.Add(seriesStyle);

        // Series
        Series s1 = new Series("S1");
        s1.StyleName = seriesStyle.Name;
        s1.Parameters["pointAreaAttributes"] = "alt=\"#[Param(y)]\"";
        s1.DataPointParameters["color"] = System.Drawing.Color.Black;

        c_chartVisits.RenderAreaMap = true;
        c_chartVisits.Series.Add(s1);
        c_chartVisits.DefineValue("S1:y", values);
        c_chartVisits.DefineValue("x", labels);
        c_chartVisits.DataBind();

        SeriesLabels sLabels = s1.CreateAnnotation("y", "CustomStyle");
        for (int i = 0; i < sLabels.Count; i++)
        {
            DataPointLabel label = sLabels[i];
            decimal value = Convert.ToDecimal(label.Text);
            label.Text = value.ToString(currency.GetNumberFormatInfo(CultureInfo.CurrentUICulture));
        }

        // Y axis
        AxisAnnotation yAnnotation = c_chartVisits.CoordinateSystem.YAxis.AxisAnnotations["Y@Xmin"];
        //yAnnotation.AxisTitle = CurrentModule.Strings.GetValue("Visits", FoundationContext.LanguageID, true);
        yAnnotation.AxisTitleStyleName = "xStyle";
        yAnnotation.LabelStyleName = "xStyle";
        yAnnotation.AxisTitleOffsetPts = 35;
        foreach (AxisLabel label in yAnnotation.Labels)
        {
            decimal value;
            if (decimal.TryParse(label.Text, out value))
            {
                label.Text = value.ToString(currency.GetNumberFormatInfo(CultureInfo.CurrentUICulture));
            }
        }

        // X axis
        AxisAnnotation xAnnotation = c_chartVisits.CoordinateSystem.XAxis.AxisAnnotations["X@Ymin"];
        xAnnotation.AxisTitleStyleName = "xStyle";
        xAnnotation.LabelStyleName = "xStyle";
		xAnnotation.HOffset = 3;
		xAnnotation.VOffset = 0;
        xAnnotation.RotationAngle = 45;


        // Background
        StripSet yStripSet1 = c_chartVisits.CoordinateSystem.PlaneXY.StripSets["YinPlaneXY"];
        yStripSet1.Color = Color.White;
        yStripSet1.AlternateColor = Color.FromArgb(242, 242, 242);
        c_chartVisits.Frame.FrameKind = ChartFrameKind.Thin2DFrame;
        c_chartVisits.Frame.FrameColor = Color.Gray;
        c_chartVisits.BackGradientKind = GradientKind.Center;
    }

    /// <summary>
    /// Gets or sets the last link ID.
    /// </summary>
    /// <value>The last link ID.</value>
    private string LastLinkID
    {
        get { return (string)ViewState["LastLinkID"]; }
        set { ViewState["LastLinkID"] = value; }
    }

    private void RemoveLastLink()
    {
        c_linkUpdatePanel.Update();
        if (LastLinkID != null)
        {
            LinkButton lastButton = FindControl(LastLinkID) as LinkButton;
            if (lastButton != null)
            {
                lastButton.CssClass = "SelectedLeftLinkReport";
            }
            LastLinkID = null;
        }
    }
}
