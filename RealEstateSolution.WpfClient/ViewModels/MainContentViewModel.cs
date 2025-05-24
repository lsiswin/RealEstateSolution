using Prism.Mvvm;
using RealEstateSolution.WpfClient.Models;
using System.Collections.ObjectModel;

namespace RealEstateSolution.WpfClient.ViewModels
{
    public class MainContentViewModel : BindableBase
    {
        private ObservableCollection<StatisticItem> _statistics;
        public ObservableCollection<StatisticItem> Statistics
        {
            get => _statistics;
            set => SetProperty(ref _statistics, value);
        }

        private ObservableCollection<QuickActionItem> _quickActions;
        public ObservableCollection<QuickActionItem> QuickActions
        {
            get => _quickActions;
            set => SetProperty(ref _quickActions, value);
        }

        private ObservableCollection<HouseItem> _houses;
        public ObservableCollection<HouseItem> Houses
        {
            get => _houses;
            set => SetProperty(ref _houses, value);
        }

        public MainContentViewModel()
        {
            // 初始化数据
            InitializeData();
        }

        private void InitializeData()
        {
            // 统计数据
            Statistics = new ObservableCollection<StatisticItem>
            {
                new StatisticItem { Label = "总房源", Value = "1,234", Trend = 5.2 },
                new StatisticItem { Label = "本月成交", Value = "89", Trend = -2.1 },
                new StatisticItem { Label = "意向客户", Value = "456", Trend = 8.5 },
                new StatisticItem { Label = "收入(万)", Value = "234.5", Trend = 12.3 }
            };

            // 快捷功能
            QuickActions = new ObservableCollection<QuickActionItem>
            {
                new QuickActionItem { Title = "发布房源", Description = "快速发布新房源信息", IconKind = "HomeAdd" },
                new QuickActionItem { Title = "客户管理", Description = "管理意向客户信息", IconKind = "AccountGroup" },
                new QuickActionItem { Title = "数据报表", Description = "查看业务统计数据", IconKind = "ChartBar" }
            };

            // 房源列表
            Houses = new ObservableCollection<HouseItem>
            {
                new HouseItem
                {
                    Title = "阳光花园 3室2厅",
                    Area = 120,
                    Rooms = "3室2厅2卫",
                    Price = 180,
                    IsUrgent = true,
                    ImageUrl = "https://example.com/house1.jpg"
                },
                new HouseItem
                {
                    Title = "城市公寓 2室1厅",
                    Area = 89,
                    Rooms = "2室1厅1卫",
                    Price = 120,
                    IsUrgent = false,
                    ImageUrl = "https://example.com/house2.jpg"
                }
            };
        }
    }

    public class StatisticItem
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public double Trend { get; set; }
    }

    public class QuickActionItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconKind { get; set; }
    }

    public class HouseItem
    {
        public string Title { get; set; }
        public double Area { get; set; }
        public string Rooms { get; set; }
        public double Price { get; set; }
        public bool IsUrgent { get; set; }
        public string ImageUrl { get; set; }
    }
} 