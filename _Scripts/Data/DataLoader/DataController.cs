using DataLoader.VO;
public class DataController : Singleton<DataController>
{
    private SoldiersVO soldiers_VO;
    private ShopVO shop_VO;
    private SkillsVO skillsVO;
    private UserDataVO userData_VO;
    private PlayerVO player_VO;
    private SeatVO seat_VO;
    private MissionVO mission_VO;
    private QuizVO _quizVO;
    private TeleportVO _teleportVO;
    private ItemInventoryVO _item_inventory_VO;
    private CharacterVO _character_VO;
    private NPCConversationVO nPC_Conversation_VO;
    private MainmenuCharacter3DVO _mainmenu_character3D_VO;
    private ColorVO color_VO;


    public QuizVO QuizVO
    {
        get
        {
            if (_quizVO == null)
                _quizVO = new QuizVO();
            return _quizVO;
        }
    }

    public MissionVO Mission_VO
    {
        get
        {
            if (mission_VO == null)
                mission_VO = new MissionVO();
            return mission_VO;
        }
    }

    public TeleportVO TeleportVO
    {
        get
        {
            if (_teleportVO == null)
            {
                _teleportVO = new TeleportVO();
            }
            return _teleportVO;
        }
    }

    public SeatVO Seat_VO
    {
        get
        {
            if (seat_VO == null)
                seat_VO = new SeatVO();
            return seat_VO;
        }
    }

    public PlayerVO Player_VO
    {
        get
        {
            if (player_VO == null)
                player_VO = new PlayerVO();
            return player_VO;
        }
    }

    public NPCConversationVO NPC_Conversation_VO
    {
        get
        {
            if (nPC_Conversation_VO == null)
            {
                nPC_Conversation_VO = new NPCConversationVO();
            }
            return nPC_Conversation_VO;
        }
    }

    public UserDataVO UserDataVo
    {
        get
        {
            if (userData_VO == null)
                userData_VO = new UserDataVO();
            return userData_VO;
        }
    }

    public SkillsVO SkillsVO
    {
        get
        {
            if (skillsVO == null)
                skillsVO = new SkillsVO();
            return skillsVO;
        }
    }

    public ShopVO ShopVO
    {
        get
        {
            if (shop_VO == null)
                shop_VO = new ShopVO();
            return shop_VO;
        }
    }

    public SoldiersVO soldiersVO
    {
        get
        {
            if (soldiers_VO == null)
                soldiers_VO = new SoldiersVO();
            return soldiers_VO;
        }
    }

    public ItemInventoryVO itemInventoryVO
    {
        get
        {
            if (_item_inventory_VO == null)
                _item_inventory_VO = new ItemInventoryVO();
            return _item_inventory_VO;
        }
    }

    public CharacterVO characterVO
    {
        get
        {
            if (_character_VO == null)
                _character_VO = new CharacterVO();
            return _character_VO;
        }
    }

    public MainmenuCharacter3DVO mainmenuCharacter3DVO
    {
        get
        {
            if (_mainmenu_character3D_VO == null)
                _mainmenu_character3D_VO = new MainmenuCharacter3DVO();
            return _mainmenu_character3D_VO;
        }
    }
    public ColorVO ColorVO
    {
        get
        {
            if (color_VO == null)
                color_VO = new ColorVO();
            return color_VO;
        }
    }
}
