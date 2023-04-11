using ChainAbstractions.Stacks;
using StacksForce;
using UnityEngine;

public class NFTUIManager : MonoBehaviour
{
    [SerializeField] GameObject nftPrefab;

    async void Start()
    {
        // initialize SDK
        ForceSDK.Init();

        // read wallet info for given STX address
        var address = "SP136AXDAQ41R31GJWJX8KX14E2T4K8PA08NCE6Q5";
        var walletInfo = StacksAbstractions.MainNet.GetWalletInfoForAddress(address);

        // get nft stream, reading nfts can be a long process...
        var nftStream = walletInfo.GetNFTs(null);
        while (Application.isPlaying)
        {
            // add nfts one by one
            var nftInfos = await nftStream.ReadMoreAsync(1);
            if (nftInfos == null || nftInfos.Count == 0) // error or stream end
                break;

            var obj = Instantiate(nftPrefab);
            obj.GetComponent<NftSpriteProvider>().NFT = nftInfos[0];
        }
    }   
}
