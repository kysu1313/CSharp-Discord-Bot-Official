using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Charts;
using BotApi.Helpers;
using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Components;

namespace BotDash.ComponentModels
{
    public class UsageGraphModel : ComponentBase
    {
        private List<ServerModel> _servers;
        private Helper _helper;
        public LineChart<double> _lineChart;
        
        protected override async Task OnInitializedAsync()
        {
            _servers = await _helper.getAllServerModels();
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await HandleRedraw();
            }
        }

        public async Task HandleRedraw()
        {
            await _lineChart.Clear();

            await _lineChart.AddLabelsDatasetsAndUpdate( _labels, GetLineChartDataset() );
        }

        LineChartDataset<double> GetLineChartDataset()
        {
            return new LineChartDataset<double>
            {
                Label = "# of randoms",
                Data = RandomizeData(),
                BackgroundColor = _backgroundColors,
                BorderColor = _borderColors,
                Fill = true,
                PointRadius = 2,
                BorderDash = new List<int> { }
            };
        }

        string[] _labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };
        List<string> _backgroundColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 0.2f ), ChartColor.FromRgba( 54, 162, 235, 0.2f ), ChartColor.FromRgba( 255, 206, 86, 0.2f ), ChartColor.FromRgba( 75, 192, 192, 0.2f ), ChartColor.FromRgba( 153, 102, 255, 0.2f ), ChartColor.FromRgba( 255, 159, 64, 0.2f ) };
        List<string> _borderColors = new List<string> { ChartColor.FromRgba( 255, 99, 132, 1f ), ChartColor.FromRgba( 54, 162, 235, 1f ), ChartColor.FromRgba( 255, 206, 86, 1f ), ChartColor.FromRgba( 75, 192, 192, 1f ), ChartColor.FromRgba( 153, 102, 255, 1f ), ChartColor.FromRgba( 255, 159, 64, 1f ) };

        List<double> RandomizeData()
        {
            var r = new Random( DateTime.Now.Millisecond );

            return new List<double> { r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble(), r.Next( 3, 50 ) * r.NextDouble() };
        }
    }
}