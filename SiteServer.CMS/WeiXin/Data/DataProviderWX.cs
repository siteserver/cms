using SiteServer.CMS.WeiXin.Provider;

namespace SiteServer.CMS.WeiXin.Data
{
	public class DataProviderWx
	{

        private static MenuDao _menuDao;
        public static MenuDao MenuDao
        {
            get
            {
                if (_menuDao == null)
                {
                    _menuDao = new MenuDao();
                }
                return _menuDao;
            }
        }

        private static WebMenuDao _webMenuDao;
        public static WebMenuDao WebMenuDao
        {
            get
            {
                if (_webMenuDao == null)
                {
                    _webMenuDao = new WebMenuDao();
                }
                return _webMenuDao;
            }
        }

        private static KeywordDao _keywordDao;
        public static KeywordDao KeywordDao
        {
            get
            {
                if (_keywordDao == null)
                {
                    _keywordDao = new KeywordDao();
                }
                return _keywordDao;
            }
        }

        private static KeywordGroupDao _keywordGroupDao;
        public static KeywordGroupDao KeywordGroupDao
        {
            get
            {
                if (_keywordGroupDao == null)
                {
                    _keywordGroupDao = new KeywordGroupDao();
                }
                return _keywordGroupDao;
            }
        }

        private static KeywordResourceDao _keywordResourceDao;
        public static KeywordResourceDao KeywordResourceDao
        {
            get
            {
                if (_keywordResourceDao == null)
                {
                    _keywordResourceDao = new KeywordResourceDao();
                }
                return _keywordResourceDao;
            }
        }

        private static KeywordMatchDao _keywordMatchDao;
        public static KeywordMatchDao KeywordMatchDao
        {
            get
            {
                if (_keywordMatchDao == null)
                {
                    _keywordMatchDao = new KeywordMatchDao();
                }
                return _keywordMatchDao;
            }
        }

        private static CountDao _countDao;
        public static CountDao CountDao
        {
            get
            {
                if (_countDao == null)
                {
                    _countDao = new CountDao();
                }
                return _countDao;
            }
        }

        private static CouponDao _couponDao;
        public static CouponDao CouponDao
        {
            get
            {
                if (_couponDao == null)
                {
                    _couponDao = new CouponDao();
                }
                return _couponDao;
            }
        }

	    private static CouponSnDao _couponSnDao;
        public static CouponSnDao CouponSnDao
        {
            get
            {
                if (_couponSnDao == null)
                {
                    _couponSnDao = new CouponSnDao();
                }
                return _couponSnDao;
            }
        }

        private static CouponActDao _couponActDao;
        public static CouponActDao CouponActDao
        {
            get
            {
                if (_couponActDao == null)
                {
                    _couponActDao = new CouponActDao();
                }
                return _couponActDao;
            }
        }

        private static AccountDao _accountDao;
        public static AccountDao AccountDao
        {
            get
            {
                if (_accountDao == null)
                {
                    _accountDao = new AccountDao();
                }
                return _accountDao;
            }
        }

        private static VoteDao _voteDao;
        public static VoteDao VoteDao
        {
            get
            {
                if (_voteDao == null)
                {
                    _voteDao = new VoteDao();
                }
                return _voteDao;
            }
        }

        private static VoteItemDao _voteItemDao;
        public static VoteItemDao VoteItemDao
        {
            get
            {
                if (_voteItemDao == null)
                {
                    _voteItemDao = new VoteItemDao();
                }
                return _voteItemDao;
            }
        }

        private static VoteLogDao _voteLogDao;
        public static VoteLogDao VoteLogDao
        {
            get
            {
                if (_voteLogDao == null)
                {
                    _voteLogDao = new VoteLogDao();
                }
                return _voteLogDao;
            }
        }

        private static MessageDao _messageDao;
        public static MessageDao MessageDao
        {
            get
            {
                if (_messageDao == null)
                {
                    _messageDao = new MessageDao();
                }
                return _messageDao;
            }
        }

        private static MessageContentDao _messageContentDao;
        public static MessageContentDao MessageContentDao
        {
            get
            {
                if (_messageContentDao == null)
                {
                    _messageContentDao = new MessageContentDao();
                }
                return _messageContentDao;
            }
        }

        private static View360Dao _view360Dao;
        public static View360Dao View360Dao
        {
            get
            {
                if (_view360Dao == null)
                {
                    _view360Dao = new View360Dao();
                }
                return _view360Dao;
            }
        }

        private static ConferenceDao _conferenceDao;
        public static ConferenceDao ConferenceDao
        {
            get
            {
                if (_conferenceDao == null)
                {
                    _conferenceDao = new ConferenceDao();
                }
                return _conferenceDao;
            }
        }

        private static ConferenceContentDao _conferenceContentDao;
        public static ConferenceContentDao ConferenceContentDao
        {
            get
            {
                if (_conferenceContentDao == null)
                {
                    _conferenceContentDao = new ConferenceContentDao();
                }
                return _conferenceContentDao;
            }
        }

        private static MapDao _mapDao;
        public static MapDao MapDao
        {
            get
            {
                if (_mapDao == null)
                {
                    _mapDao = new MapDao();
                }
                return _mapDao;
            }
        }

        private static LotteryDao _lotteryDao;
        public static LotteryDao LotteryDao
        {
            get
            {
                if (_lotteryDao == null)
                {
                    _lotteryDao = new LotteryDao();
                }
                return _lotteryDao;
            }
        }

        private static LotteryAwardDao _lotteryAwardDao;
        public static LotteryAwardDao LotteryAwardDao
        {
            get
            {
                if (_lotteryAwardDao == null)
                {
                    _lotteryAwardDao = new LotteryAwardDao();
                }
                return _lotteryAwardDao;
            }
        }

        private static LotteryWinnerDao _lotteryWinnerDao;
        public static LotteryWinnerDao LotteryWinnerDao
        {
            get
            {
                if (_lotteryWinnerDao == null)
                {
                    _lotteryWinnerDao = new LotteryWinnerDao();
                }
                return _lotteryWinnerDao;
            }
        }

        private static LotteryLogDao _lotteryLogDao;
        public static LotteryLogDao LotteryLogDao
        {
            get
            {
                if (_lotteryLogDao == null)
                {
                    _lotteryLogDao = new LotteryLogDao();
                }
                return _lotteryLogDao;
            }
        }

        private static AlbumDao _albumDao;
        public static AlbumDao AlbumDao
        {
            get
            {
                if (_albumDao == null)
                {
                    _albumDao = new AlbumDao();
                }
                return _albumDao;
            }
        }

        private static AlbumContentDao _albumContentDao;
        public static AlbumContentDao AlbumContentDao
        {
            get
            {
                if (_albumContentDao == null)
                {
                    _albumContentDao = new AlbumContentDao();
                }
                return _albumContentDao;
            }
        }

        private static AppointmentDao _appointmentDao;
        public static AppointmentDao AppointmentDao
        {
            get
            {
                if (_appointmentDao == null)
                {
                    _appointmentDao = new AppointmentDao();
                }
                return _appointmentDao;
            }
        }

        private static AppointmentItemDao _appointmentItemDao;
        public static AppointmentItemDao AppointmentItemDao
        {
            get
            {
                if (_appointmentItemDao == null)
                {
                    _appointmentItemDao = new AppointmentItemDao();
                }
                return _appointmentItemDao;
            }
        }

        private static AppointmentContentDao _appointmentContentDao;
        public static AppointmentContentDao AppointmentContentDao
        {
            get
            {
                if (_appointmentContentDao == null)
                {
                    _appointmentContentDao = new AppointmentContentDao();
                }
                return _appointmentContentDao;
            }
        }

        private static SearchDao _searchDao;
        public static SearchDao SearchDao
        {
            get
            {
                if (_searchDao == null)
                {
                    _searchDao = new SearchDao();
                }
                return _searchDao;
            }
        }

        private static SearchNavigationDao _searchNavigationDao;
        public static SearchNavigationDao SearchNavigationDao
        {
            get
            {
                if (_searchNavigationDao == null)
                {
                    _searchNavigationDao = new SearchNavigationDao();
                }
                return _searchNavigationDao;
            }
        }

        private static StoreDao _storeDao;
        public static StoreDao StoreDao
        {
            get
            {
                if (_storeDao == null)
                {
                    _storeDao = new StoreDao();
                }
                return _storeDao;
            }
        }

        private static StoreCategoryDao _storeCategoryDao;
        public static StoreCategoryDao StoreCategoryDao
        {
            get
            {
                if (_storeCategoryDao == null)
                {
                    _storeCategoryDao = new StoreCategoryDao();
                }
                return _storeCategoryDao;
            }
        }

        private static StoreItemDao _storeItemDao;
        public static StoreItemDao StoreItemDao
        {
            get
            {
                if (_storeItemDao == null)
                {
                    _storeItemDao = new StoreItemDao();
                }
                return _storeItemDao;
            }
        }

        private static ScenceDao _scenceDao;
        public static ScenceDao ScenceDao
        {
            get
            {
                if (_scenceDao == null)
                {
                    _scenceDao = new ScenceDao();
                }
                return _scenceDao;
            }
        }

        private static ConfigExtendDao _configExtendDao;
        public static ConfigExtendDao ConfigExtendDao
        {
            get
            {
                if (_configExtendDao == null)
                {
                    _configExtendDao = new ConfigExtendDao();
                }
                return _configExtendDao;
            }
        }

        private static CardDao _cardDao;
        public static CardDao CardDao
        {
            get
            {
                if (_cardDao == null)
                {
                    _cardDao = new CardDao();
                }
                return _cardDao;
            }
        }

        private static CardSnDao _cardSnDao;
        public static CardSnDao CardSnDao
        {
            get
            {
                if (_cardSnDao == null)
                {
                    _cardSnDao = new CardSnDao();
                }
                return _cardSnDao;
            }
        }

        private static CardEntitySnDao _cardEntitySnDao;
        public static CardEntitySnDao CardEntitySnDao
        {
            get
            {
                if (_cardEntitySnDao == null)
                {
                    _cardEntitySnDao = new CardEntitySnDao();
                }
                return _cardEntitySnDao;
            }
        }

        private static CardSignLogDao _cardSignLogDao;
        public static CardSignLogDao CardSignLogDao
        {
            get
            {
                if (_cardSignLogDao == null)
                {
                    _cardSignLogDao = new CardSignLogDao();
                }
                return _cardSignLogDao;
            }
        }

        private static CardCashLogDao _cardCashLogDao;
        public static CardCashLogDao CardCashLogDao
        {
            get
            {
                if (_cardCashLogDao == null)
                {
                    _cardCashLogDao = new CardCashLogDao();
                }
                return _cardCashLogDao;
            }
        }

        private static CollectDao _collectDao;
        public static CollectDao CollectDao
        {
            get
            {
                if (_collectDao == null)
                {
                    _collectDao = new CollectDao();
                }
                return _collectDao;
            }
        }

        private static CollectItemDao _collectItemDao;
        public static CollectItemDao CollectItemDao
        {
            get
            {
                if (_collectItemDao == null)
                {
                    _collectItemDao = new CollectItemDao();
                }
                return _collectItemDao;
            }
        }

        private static CollectLogDao _collectLogDao;
        public static CollectLogDao CollectLogDao
        {
            get
            {
                if (_collectLogDao == null)
                {
                    _collectLogDao = new CollectLogDao();
                }
                return _collectLogDao;
            }
        }
	}
}
