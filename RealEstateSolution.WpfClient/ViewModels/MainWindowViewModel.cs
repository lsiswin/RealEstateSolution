using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace RealEstateSolution.WpfClient.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public ObservableCollection<StatisticItem> Statistics { get; } = new()
        {
            new StatisticItem { Label = "本月新增房源", Value = "128", Trend = 12.5 },
            new StatisticItem { Label = "带看预约数", Value = "56", Trend = 8.3 },
            new StatisticItem { Label = "成交总额", Value = "￥3,680万", Trend = 15.2 },
            new StatisticItem { Label = "新增客户", Value = "86", Trend = -5.4 }
        };

        public ObservableCollection<QuickAction> QuickActions { get; } = new()
        {
            new QuickAction { Title = "登记房源", Description = "快速录入新房源信息", IconKind = "Home" },
            new QuickAction { Title = "预约管理", Description = "查看和安排看房日程", IconKind = "Calendar" },
            new QuickAction { Title = "客户跟进", Description = "及时响应客户需求", IconKind = "MessageText" }
        };

        public ObservableCollection<HouseItem> Houses { get; } = new()
        {
            new HouseItem
            {
                Title = "华府骏苑 3室2厅",
                Area = 128,
                Rooms = "3室2厅2卫",
                Price = 580,
                IsUrgent = true,
                ImageUrl = "https://ai-public.mastergo.com/ai/img_res/fcd34b9d2caa9e9f6083194632e3956d.jpg"
            },
            // 添加更多房源...
        };

        public DelegateCommand SearchCommand { get; }
        public DelegateCommand AddHouseCommand { get; }

        public MainWindowViewModel()
        {
            SearchCommand = new DelegateCommand(ExecuteSearch);
            AddHouseCommand = new DelegateCommand(ExecuteAddHouse);
        }

        private void ExecuteSearch()
        {
            // 实现搜索逻辑
        }

        private void ExecuteAddHouse()
        {
            // 实现添加房源逻辑
        }
    }

    public class QuickAction
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconKind { get; set; }
    }

} 