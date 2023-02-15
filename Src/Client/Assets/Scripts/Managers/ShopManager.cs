using Common.Data;
using Models;
using Services;

namespace Managers
{
    /// <summary>
    /// 商店管理器
    /// </summary>
    public class ShopManager : Singleton<ShopManager>
    {
        private UIShop uiShop;

        public bool isClick = true;
       
        public void Init()
        {
            NpcManagerMe.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
        }

        #region 点击NPC打开商店
        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }

        private void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId, out shop))
            {
                uiShop = UIManagerMe.Instance.Show<UIShop>();
                if (uiShop != null)
                {
                    uiShop.SetShop(shop);
                }
            }
        }
        #endregion

        public bool BuyItem(int shopId, int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }

        internal void SetMoney()
        {
            if(uiShop != null)
            {
                this.uiShop.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
            }
        }
    }
}
