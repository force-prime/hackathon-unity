using ChainAbstractions;
using ChainAbstractions.Stacks;
using ChainAbstractions.Stacks.ContractWrappers;
using StacksForce;
using System.Numerics;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TMP_Text yourAddress;
    [SerializeField] TMP_Text yourBalance;

    [SerializeField] TMP_Text prize;
    [SerializeField] TMP_Text owner;

    [SerializeField] TMP_Text nftName;
    [SerializeField] NftSpriteProvider image;

    // TODO: fill or input mnemonic
    private const string MNEMONIC = "";
    private const string FULL_TOKEN_ID = "SP2KBR4J4VNKKT63319EQV0JHXM6PH61AX9BPW748.force-prime-hackathon-nft-1::STEAL-ME";
    private const string CONTRACT_ADDRESS = "SP2KBR4J4VNKKT63319EQV0JHXM6PH61AX9BPW748.force-prime-hackathon-nft-1";

    private static IBlockchain Chain = StacksAbstractions.MainNet;

    private IWallet wallet;

    private void Awake()
    {
        // required step - initialize SDK 
        ForceSDK.Init();
        // initialize wallet from the seed phrase
        wallet = Chain.GetWalletForMnemonic(MNEMONIC);
    }

    private void Start()
    {
        Refresh();
    }

    public async void TryToSteal()
    {
        // prepare transaction that calls 'steal' method with no prameters
        var stealTransaction = await wallet.GetContractCallTransaction(
            CONTRACT_ADDRESS,
            "steal",
            new System.Collections.Generic.List<IVariable> { });

        // if transaction is prepared and it's cost is not too high then send it!
        if (stealTransaction.Error == null && stealTransaction.Cost.Balance < 200000)
        {
            var sendResult = await stealTransaction.Send();
            if (sendResult != null)
                Debug.LogError("Send result error: " + sendResult);
            else
                Debug.Log("Steal transaction sent!");
        }
    }

    public async void Refresh()
    {
        if (wallet != null)
        {
            yourAddress.text = wallet.GetAddress();
            // read default token (STX)
            yourBalance.text = (await wallet.GetToken(null)).Data.ToString();

            // SIP 09 is a standard describing NFT contracts
            var sip09 = new SIP09UnsignedInteger(FULL_TOKEN_ID);
            // in our case we know that NFT has id = '1' and we ask for that nft information
            INFT nft = await sip09.GetById(1);

            if (nft != null)
            {
                image.NFT = nft;
                nftName.text = nft.Name;
            }

            var currentOwner = await sip09.GetTokenOwner(1);
            if (currentOwner.IsSuccess)
            {
                owner.text = string.IsNullOrEmpty(currentOwner.Data) ? "NONE" : currentOwner.Data.ToString();
            } else
            {
                Debug.LogError(currentOwner.Error);
            }

            var currentPrize = await Chain.CallReadOnly(wallet.GetAddress(), CONTRACT_ADDRESS, "get-prize");
            // format returned value (in uSTX) to pretty value
            if (currentPrize.IsSuccess)
                prize.text = StacksAbstractions.Stx.FormatCount(currentPrize.Data.GetValue<BigInteger>());
        }
    }
}
