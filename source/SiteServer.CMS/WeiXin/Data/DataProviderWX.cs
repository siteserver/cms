using SiteServer.CMS.WeiXin.Provider;

namespace SiteServer.CMS.WeiXin.Data
{
	public class DataProviderWX
	{

        private static MenuDAO menuDAO;
        public static MenuDAO MenuDAO
        {
            get
            {
                if (menuDAO == null)
                {
                    menuDAO = new MenuDAO();
                }
                return menuDAO;
            }
        }

        private static WebMenuDAO webMenuDAO;
        public static WebMenuDAO WebMenuDAO
        {
            get
            {
                if (webMenuDAO == null)
                {
                    webMenuDAO = new WebMenuDAO();
                }
                return webMenuDAO;
            }
        }

        private static KeywordDAO keywordDAO;
        public static KeywordDAO KeywordDAO
        {
            get
            {
                if (keywordDAO == null)
                {
                    keywordDAO = new KeywordDAO();
                }
                return keywordDAO;
            }
        }

        private static KeywordGroupDAO keywordGroupDAO;
        public static KeywordGroupDAO KeywordGroupDAO
        {
            get
            {
                if (keywordGroupDAO == null)
                {
                    keywordGroupDAO = new KeywordGroupDAO();
                }
                return keywordGroupDAO;
            }
        }

        private static KeywordResourceDAO keywordResourceDAO;
        public static KeywordResourceDAO KeywordResourceDAO
        {
            get
            {
                if (keywordResourceDAO == null)
                {
                    keywordResourceDAO = new KeywordResourceDAO();
                }
                return keywordResourceDAO;
            }
        }

        private static KeywordMatchDAO keywordMatchDAO;
        public static KeywordMatchDAO KeywordMatchDAO
        {
            get
            {
                if (keywordMatchDAO == null)
                {
                    keywordMatchDAO = new KeywordMatchDAO();
                }
                return keywordMatchDAO;
            }
        }

        private static CountDAO countDAO;
        public static CountDAO CountDAO
        {
            get
            {
                if (countDAO == null)
                {
                    countDAO = new CountDAO();
                }
                return countDAO;
            }
        }

        private static CouponDAO couponDAO;
        public static CouponDAO CouponDAO
        {
            get
            {
                if (couponDAO == null)
                {
                    couponDAO = new CouponDAO();
                }
                return couponDAO;
            }
        }

        private static CouponSNDAO couponSNDAO;
        public static CouponSNDAO CouponSNDAO
        {
            get
            {
                if (couponSNDAO == null)
                {
                    couponSNDAO = new CouponSNDAO();
                }
                return couponSNDAO;
            }
        }

        private static CouponActDAO couponActDAO;
        public static CouponActDAO CouponActDAO
        {
            get
            {
                if (couponActDAO == null)
                {
                    couponActDAO = new CouponActDAO();
                }
                return couponActDAO;
            }
        }

        private static AccountDAO accountDAO;
        public static AccountDAO AccountDAO
        {
            get
            {
                if (accountDAO == null)
                {
                    accountDAO = new AccountDAO();
                }
                return accountDAO;
            }
        }

        private static VoteDAO voteDAO;
        public static VoteDAO VoteDAO
        {
            get
            {
                if (voteDAO == null)
                {
                    voteDAO = new VoteDAO();
                }
                return voteDAO;
            }
        }

        private static VoteItemDAO voteItemDAO;
        public static VoteItemDAO VoteItemDAO
        {
            get
            {
                if (voteItemDAO == null)
                {
                    voteItemDAO = new VoteItemDAO();
                }
                return voteItemDAO;
            }
        }

        private static VoteLogDAO voteLogDAO;
        public static VoteLogDAO VoteLogDAO
        {
            get
            {
                if (voteLogDAO == null)
                {
                    voteLogDAO = new VoteLogDAO();
                }
                return voteLogDAO;
            }
        }

        private static MessageDAO messageDAO;
        public static MessageDAO MessageDAO
        {
            get
            {
                if (messageDAO == null)
                {
                    messageDAO = new MessageDAO();
                }
                return messageDAO;
            }
        }

        private static MessageContentDAO messageContentDAO;
        public static MessageContentDAO MessageContentDAO
        {
            get
            {
                if (messageContentDAO == null)
                {
                    messageContentDAO = new MessageContentDAO();
                }
                return messageContentDAO;
            }
        }

        private static View360DAO view360DAO;
        public static View360DAO View360DAO
        {
            get
            {
                if (view360DAO == null)
                {
                    view360DAO = new View360DAO();
                }
                return view360DAO;
            }
        }

        private static ConferenceDAO conferenceDAO;
        public static ConferenceDAO ConferenceDAO
        {
            get
            {
                if (conferenceDAO == null)
                {
                    conferenceDAO = new ConferenceDAO();
                }
                return conferenceDAO;
            }
        }

        private static ConferenceContentDAO conferenceContentDAO;
        public static ConferenceContentDAO ConferenceContentDAO
        {
            get
            {
                if (conferenceContentDAO == null)
                {
                    conferenceContentDAO = new ConferenceContentDAO();
                }
                return conferenceContentDAO;
            }
        }

        private static MapDAO mapDAO;
        public static MapDAO MapDAO
        {
            get
            {
                if (mapDAO == null)
                {
                    mapDAO = new MapDAO();
                }
                return mapDAO;
            }
        }

        private static LotteryDAO lotteryDAO;
        public static LotteryDAO LotteryDAO
        {
            get
            {
                if (lotteryDAO == null)
                {
                    lotteryDAO = new LotteryDAO();
                }
                return lotteryDAO;
            }
        }

        private static LotteryAwardDAO lotteryAwardDAO;
        public static LotteryAwardDAO LotteryAwardDAO
        {
            get
            {
                if (lotteryAwardDAO == null)
                {
                    lotteryAwardDAO = new LotteryAwardDAO();
                }
                return lotteryAwardDAO;
            }
        }

        private static LotteryWinnerDAO lotteryWinnerDAO;
        public static LotteryWinnerDAO LotteryWinnerDAO
        {
            get
            {
                if (lotteryWinnerDAO == null)
                {
                    lotteryWinnerDAO = new LotteryWinnerDAO();
                }
                return lotteryWinnerDAO;
            }
        }

        private static LotteryLogDAO lotteryLogDAO;
        public static LotteryLogDAO LotteryLogDAO
        {
            get
            {
                if (lotteryLogDAO == null)
                {
                    lotteryLogDAO = new LotteryLogDAO();
                }
                return lotteryLogDAO;
            }
        }

        private static AlbumDAO albumDAO;
        public static AlbumDAO AlbumDAO
        {
            get
            {
                if (albumDAO == null)
                {
                    albumDAO = new AlbumDAO();
                }
                return albumDAO;
            }
        }

        private static AlbumContentDAO albumContentDAO;
        public static AlbumContentDAO AlbumContentDAO
        {
            get
            {
                if (albumContentDAO == null)
                {
                    albumContentDAO = new AlbumContentDAO();
                }
                return albumContentDAO;
            }
        }

        private static AppointmentDAO appointmentDAO;
        public static AppointmentDAO AppointmentDAO
        {
            get
            {
                if (appointmentDAO == null)
                {
                    appointmentDAO = new AppointmentDAO();
                }
                return appointmentDAO;
            }
        }

        private static AppointmentItemDAO appointmentItemDAO;
        public static AppointmentItemDAO AppointmentItemDAO
        {
            get
            {
                if (appointmentItemDAO == null)
                {
                    appointmentItemDAO = new AppointmentItemDAO();
                }
                return appointmentItemDAO;
            }
        }

        private static AppointmentContentDAO appointmentContentDAO;
        public static AppointmentContentDAO AppointmentContentDAO
        {
            get
            {
                if (appointmentContentDAO == null)
                {
                    appointmentContentDAO = new AppointmentContentDAO();
                }
                return appointmentContentDAO;
            }
        }

        private static SearchDAO searchDAO;
        public static SearchDAO SearchDAO
        {
            get
            {
                if (searchDAO == null)
                {
                    searchDAO = new SearchDAO();
                }
                return searchDAO;
            }
        }

        private static SearchNavigationDAO searchNavigationDAO;
        public static SearchNavigationDAO SearchNavigationDAO
        {
            get
            {
                if (searchNavigationDAO == null)
                {
                    searchNavigationDAO = new SearchNavigationDAO();
                }
                return searchNavigationDAO;
            }
        }

        private static StoreDAO storeDAO;
        public static StoreDAO StoreDAO
        {
            get
            {
                if (storeDAO == null)
                {
                    storeDAO = new StoreDAO();
                }
                return storeDAO;
            }
        }

        private static StoreCategoryDAO storeCategoryDAO;
        public static StoreCategoryDAO StoreCategoryDAO
        {
            get
            {
                if (storeCategoryDAO == null)
                {
                    storeCategoryDAO = new StoreCategoryDAO();
                }
                return storeCategoryDAO;
            }
        }

        private static StoreItemDAO storeItemDAO;
        public static StoreItemDAO StoreItemDAO
        {
            get
            {
                if (storeItemDAO == null)
                {
                    storeItemDAO = new StoreItemDAO();
                }
                return storeItemDAO;
            }
        }

        private static ScenceDAO scenceDAO;
        public static ScenceDAO ScenceDAO
        {
            get
            {
                if (scenceDAO == null)
                {
                    scenceDAO = new ScenceDAO();
                }
                return scenceDAO;
            }
        }

        private static ConfigExtendDAO configExtendDAO;
        public static ConfigExtendDAO ConfigExtendDAO
        {
            get
            {
                if (configExtendDAO == null)
                {
                    configExtendDAO = new ConfigExtendDAO();
                }
                return configExtendDAO;
            }
        }

        private static CardDAO cardDAO;
        public static CardDAO CardDAO
        {
            get
            {
                if (cardDAO == null)
                {
                    cardDAO = new CardDAO();
                }
                return cardDAO;
            }
        }

        private static CardSNDAO cardSNDAO;
        public static CardSNDAO CardSNDAO
        {
            get
            {
                if (cardSNDAO == null)
                {
                    cardSNDAO = new CardSNDAO();
                }
                return cardSNDAO;
            }
        }

        private static CardEntitySNDAO cardEntitySNDAO;
        public static CardEntitySNDAO CardEntitySNDAO
        {
            get
            {
                if (cardEntitySNDAO == null)
                {
                    cardEntitySNDAO = new CardEntitySNDAO();
                }
                return cardEntitySNDAO;
            }
        }

        private static CardSignLogDAO cardSignLogDAO;
        public static CardSignLogDAO CardSignLogDAO
        {
            get
            {
                if (cardSignLogDAO == null)
                {
                    cardSignLogDAO = new CardSignLogDAO();
                }
                return cardSignLogDAO;
            }
        }

        private static CardCashLogDAO cardCashLogDAO;
        public static CardCashLogDAO CardCashLogDAO
        {
            get
            {
                if (cardCashLogDAO == null)
                {
                    cardCashLogDAO = new CardCashLogDAO();
                }
                return cardCashLogDAO;
            }
        }

        private static CollectDAO collectDAO;
        public static CollectDAO CollectDAO
        {
            get
            {
                if (collectDAO == null)
                {
                    collectDAO = new CollectDAO();
                }
                return collectDAO;
            }
        }

        private static CollectItemDAO collectItemDAO;
        public static CollectItemDAO CollectItemDAO
        {
            get
            {
                if (collectItemDAO == null)
                {
                    collectItemDAO = new CollectItemDAO();
                }
                return collectItemDAO;
            }
        }

        private static CollectLogDAO collectLogDAO;
        public static CollectLogDAO CollectLogDAO
        {
            get
            {
                if (collectLogDAO == null)
                {
                    collectLogDAO = new CollectLogDAO();
                }
                return collectLogDAO;
            }
        }
	}
}
