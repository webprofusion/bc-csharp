using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Kyber;
using Org.BouncyCastle.Pqc.Crypto.Utilities;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Pqc.Crypto.Tests
{
    [TestFixture]
    public class CrystalsKyberTest
    {
        private static readonly Dictionary<string, KyberParameters> Parameters = new Dictionary<string, KyberParameters>()
        {
            { "kyber512.rsp", KyberParameters.kyber512 },
            { "kyber768.rsp", KyberParameters.kyber768 },
            { "kyber1024.rsp", KyberParameters.kyber1024 },
            { "kyber512aes.rsp", KyberParameters.kyber512_aes },
            { "kyber768aes.rsp", KyberParameters.kyber768_aes },
            { "kyber1024aes.rsp", KyberParameters.kyber1024_aes },
        };

        private static readonly string[] TestVectorFiles =
        {
            "kyber512.rsp",
            "kyber768.rsp",
            "kyber1024.rsp",
        };

        private static readonly string[] TestVectorFilesAes =
        {
            "kyber512aes.rsp",
            "kyber768aes.rsp",
            "kyber1024aes.rsp",
        };

        [Test]
        public void TestKyber()
        {
            /*
            count = 0
            seed = 061550234D158C5EC95595FE04EF7A25767F2E24CC2BC479D09D86DC9ABCFDE7056A8C266F9EF97ED08541DBD2E1FFA1
            pk = D22302CBD3399FACC630991FC8F28BDB4354762541527678BCF61F65C241146C426D23B9BFAA6B7DF18C97F20C1B6125BF874B1D89475852C448215DB0EB7737F91480E8CEBD9A0871574F5AB62D9020175EC6927CA0B54C09818E42CF92A383172422C7DC1831D63B0C295DE75159DB8034E9E07F7B0B910C3C1E5FB66B3DC523F1FA6EB4910CB89A6C17562C83AB4C18D0CD7E0796592A372AA409B1C557347CCACDC4644A119064D06DD474929D1C6FB4D686E5491CE4BC89A30BB4B8C41BCE5157DFC1360823B1AB618C14B10F98C25067398EA7018C278A4B3DF31334D603B2044EF187CD9BC6CE42725BD962C264983E9E18155A8B9C47143D70460A26A56FE7658C1F150348C6087EF758AD167887860A007A5FC37358D43B5EBEE820ACEA474F0AC07B76802866199C61231D5C747C93774D2C1E0C1C67E6C81B82752173E125BAF39B4FD19A4F453DC57976B1D97FE6996992BBB65B7CB25D077BBAA6A13322899AF659CF1B3558C1B5001154B625809ED89AEEBB89E6EA7D67F723D045AB05715C42355DA6A5C8DD39C8ABE3037751A01ED1C7374919F3121B5A52C53D1487316769F80721DEEAAAD3C90F76E7AE9E12BA92B32B5FD457E3C752C2650DFB885771CB77AC3C785A8C562E6A1C63C2A55EA47CF8B90EB8225C123C346452566235B2F31823A33521E087937A345D8D663EEAA05658917BBAA008C2E335F8850A90A326D0E66432F44CEB8289E4ECB2D12958E984072ECACB88E1348FF0B55654ACBA5B54971CBAEBA88EC4B91A94C37192FA982BECB9F3DA421603B61A51BC8E36CBD053851C77B1B926B17A272AA9023246B02B3ED47F66A00BD5684823634E7CE58CF8F306E35B1E5322824D904801F0A2FA7C2BC9C252B0A56B7BA2AB0F636021745A70A9A43E2B0A8D615970B65309624B5184BCC30B911679AEDD76025FE3908FD67897B0CF4BE5A6F5413D7DD98564B23E42A93E4AA8821CD45054C643EDC1158DB6B3DEB13FB5A51EBD1A8A78B87225A7338E101104C4A220D9BDEDD48C85A1C2DAE781A80C40E13B87EAC73A764201C9B760CCFB1AE392699C7039D27C39362B27B8FC6F07A8A3D4410F1547C48A9997F62C61074452EF1515F8A649EBCA9437205A4E8A61606B41DAF6834D671F4D852C0C9C4096611648C6A3170678B1537CC1828D93580C9E5849A9653175ACB753F2BE7437BE45F6C603E485F2EC301BB42B6C37C225D7495A584AE231890AB5C8C35C268CF4BBB0213C096019319561A8A6947637AA40D006B415BB2CFA2237E0890B6A3BC134ABF8F6585E108D15940F91F4BF5B0C818055B21DEA6E63B553988C47F4B94E7CF800A493B4734705EDC56A4B6021C629500675876804CF0B951F038A5C7FE58E89774EF2992FD7C63099D352A7D21560B788B405709861817E59A96B3A3A83CBA803B16934331071905BBEC6532900155D8AC88CB32E4E21A3BD3A03FDEC325A51CD2773964E6784FCF1853737AA64EB67564727272661ABF84313A57A44B123C65509CFB7A6F6641CDCC3B57FE628C7B8192DB44FFBF5796A8613B1FA126F6076883C783DC24E2A4464C40B3A41CA70AE87620866CF4FCB2BD204BF5C283812BA056AC0C345E379C4BA24D750901279BB2F3A16F612BFADB35703332C7C136F68EAB6755C66B6A4AD1AABA7B768A58ACAACC10A459A1CC8EF29377BC200E4D315A30A6BCC3256F9734D06E9779CAA5442A9A16069081377C76E75154368072DC446ED6C8B8E622A21E383CF9BA1FB434E2ECC81E7B78CEE986B8FF798AB18CF9634543546284EDA2A26B47F05B735BCDB1202220076DC8B4E4B9F853533C8F6C7FF38817BA49712835785F17F14CA01D0C1C1E98810FE0B36E5B427157B9418449CEDD641A4293C85C32700102ACEC22EBAD98ED160A5F027BD4CDA57F1F3720A12C134654DD5E73F829676495390D0E7929D6034E9C55F7D55BA658BC587988E8AF94960F6CFB8D5AF7A0021535A6E25E437D49A780698BE22AC9953949F571B85A685725F8207A2B0AE849B601AB91B159B3DF4A154C2041E776070AFC42969322380917C97510799F3149131477E16663D3174C7C1CAEA788535C6C005A64F2868631B31B66E205FD38C1D84542D0F1B578F58C9BF5A0FAEAB6AB6494893053165EAFD465FC64A0C5F8F3F9003489415899D59A543D8208C54A3166529B53922
            sk = 07638FB69868F3D320E5862BD96933FEB311B362093C9B5D50170BCED43F1B536D9A204BB1F22695950BA1F2A9E8EB828B284488760B3FC84FABA04275D5628E39C5B2471374283C503299C0AB49B66B8BBB56A4186624F919A2BA59BB08D8551880C2BEFC4F87F25F59AB587A79C327D792D54C974A69262FF8A78938289E9A87B688B083E0595FE218B6BB1505941CE2E81A5A64C5AAC60417256985349EE47A52420A5F97477B7236AC76BC70E8288729287EE3E34A3DBC3683C0B7B10029FC203418537E7466BA6385A8FF301EE12708F82AAA1E380FC7A88F8F205AB7E88D7E95952A55BA20D09B79A47141D62BF6EB7DD307B08ECA13A5BC5F6B68581C6865B27BBCDDAB142F4B2CBFF488C8A22705FAA98A2B9EEA3530C76662335CC7EA3A00777725EBCCCD2A4636B2D9122FF3AB77123CE0883C1911115E50C9E8A94194E48DD0D09CFFB3ADCD2C1E92430903D07ADBF00532031575AA7F9E7B5A1F3362DEC936D4043C05F2476C07578BC9CBAF2AB4E382727AD41686A96B2548820BB03B32F11B2811AD62F489E951632ABA0D1DF89680CC8A8B53B481D92A68D70B4EA1C3A6A561C0692882B5CA8CC942A8D495AFCB06DE89498FB935B775908FE7A03E324D54CC19D4E1AABD3593B38B19EE1388FE492B43127E5A504253786A0D69AD32601C28E2C88504A5BA599706023A61363E17C6B9BB59BDC697452CD059451983D738CA3FD034E3F5988854CA05031DB09611498988197C6B30D258DFE26265541C89A4B31D6864E9389B03CB74F7EC4323FB9421A4B9790A26D17B0398A26767350909F84D57B6694DF830664CA8B3C3C03ED2AE67B89006868A68527CCD666459AB7F056671000C6164D3A7F266A14D97CBD7004D6C92CACA770B844A4FA9B182E7B18CA885082AC5646FCB4A14E1685FEB0C9CE3372AB95365C04FD83084F80A23FF10A05BF15F7FA5ACC6C0CB462C33CA524FA6B8BB359043BA68609EAA2536E81D08463B19653B5435BA946C9ADDEB202B04B031CC960DCC12E4518D428B32B257A4FC7313D3A7980D80082E934F9D95C32B0A0191A23604384DD9E079BBBAA266D14C3F756B9F2133107433A4E83FA7187282A809203A4FAF841851833D121AC383843A5E55BC2381425E16C7DB4CC9AB5C1B0D91A47E2B8DE0E582C86B6B0D907BB360B97F40AB5D038F6B75C814B27D9B968D419832BC8C2BEE605EF6E5059D33100D90485D378450014221736C07407CAC260408AA64926619788B8601C2A752D1A6CBF820D7C7A04716203225B3895B9342D147A8185CFC1BB65BA06B4142339903C0AC4651385B45D98A8B19D28CD6BAB088787F7EE1B12461766B43CBCCB96434427D93C065550688F6948ED1B5475A425F1B85209D061C08B56C1CC069F6C0A7C6F29358CAB911087732A649D27C9B98F9A48879387D9B00C25959A71654D6F6A946164513E47A75D005986C2363C09F6B537ECA78B9303A5FA457608A586A653A347DB04DFCC19175B3A301172536062A658A95277570C8852CA8973F4AE123A334047DD711C8927A634A03388A527B034BF7A8170FA702C1F7C23EC32D18A2374890BE9C787A9409C82D192C4BB705A2F996CE405D85A4C1A1AB9B6AEB49CCE1C2F8A97C3516C72A00A46263BAA696BF25727719C3216423618FF33380934A6C10545C4C5C5155B12486181FC7A2319873978B6A2A67490F8256BD2196FE1792A4C00077B812EAE8BED3572499684AB3371876761E450C9F9D2768A36806D7AB2046C91F17599E9AC592990808DCD7B4D0919072F14EC361773B7252444C323C308326F4A30F8680D2F748F56A132B82674ED0184620B82AD2CB182C97B481626647491290A011CC73828685A8C367A5B9CF8D621B0D5C1EFF03172758BD004978C251CD51342228989CAE6332AC486437CB5C57D4307462865253BE217B3515C73DF405B7F28217AD0B8CF60C2FFFAA0A0048B1FB4ACDCDC38B5250CFEC356A6DE26CFA7A588FDC86F98C854AC64C7BFAA96F5A32CC0610934BAA6A586B9A2054F13BA274174AA0D2B3A81B96A940666F789B5A6BCDC0A6A0178A0C9A02578A493F6EEA0D2E6C13951C9F249A5E8DD71DD49A742D451F1ABBA19AF8C547855E0AFC728E90ABB499C9BEEB766F4729CDA22263E324D22302CBD3399FACC630991FC8F28BDB4354762541527678BCF61F65C241146C426D23B9BFAA6B7DF18C97F20C1B6125BF874B1D89475852C448215DB0EB7737F91480E8CEBD9A0871574F5AB62D9020175EC6927CA0B54C09818E42CF92A383172422C7DC1831D63B0C295DE75159DB8034E9E07F7B0B910C3C1E5FB66B3DC523F1FA6EB4910CB89A6C17562C83AB4C18D0CD7E0796592A372AA409B1C557347CCACDC4644A119064D06DD474929D1C6FB4D686E5491CE4BC89A30BB4B8C41BCE5157DFC1360823B1AB618C14B10F98C25067398EA7018C278A4B3DF31334D603B2044EF187CD9BC6CE42725BD962C264983E9E18155A8B9C47143D70460A26A56FE7658C1F150348C6087EF758AD167887860A007A5FC37358D43B5EBEE820ACEA474F0AC07B76802866199C61231D5C747C93774D2C1E0C1C67E6C81B82752173E125BAF39B4FD19A4F453DC57976B1D97FE6996992BBB65B7CB25D077BBAA6A13322899AF659CF1B3558C1B5001154B625809ED89AEEBB89E6EA7D67F723D045AB05715C42355DA6A5C8DD39C8ABE3037751A01ED1C7374919F3121B5A52C53D1487316769F80721DEEAAAD3C90F76E7AE9E12BA92B32B5FD457E3C752C2650DFB885771CB77AC3C785A8C562E6A1C63C2A55EA47CF8B90EB8225C123C346452566235B2F31823A33521E087937A345D8D663EEAA05658917BBAA008C2E335F8850A90A326D0E66432F44CEB8289E4ECB2D12958E984072ECACB88E1348FF0B55654ACBA5B54971CBAEBA88EC4B91A94C37192FA982BECB9F3DA421603B61A51BC8E36CBD053851C77B1B926B17A272AA9023246B02B3ED47F66A00BD5684823634E7CE58CF8F306E35B1E5322824D904801F0A2FA7C2BC9C252B0A56B7BA2AB0F636021745A70A9A43E2B0A8D615970B65309624B5184BCC30B911679AEDD76025FE3908FD67897B0CF4BE5A6F5413D7DD98564B23E42A93E4AA8821CD45054C643EDC1158DB6B3DEB13FB5A51EBD1A8A78B87225A7338E101104C4A220D9BDEDD48C85A1C2DAE781A80C40E13B87EAC73A764201C9B760CCFB1AE392699C7039D27C39362B27B8FC6F07A8A3D4410F1547C48A9997F62C61074452EF1515F8A649EBCA9437205A4E8A61606B41DAF6834D671F4D852C0C9C4096611648C6A3170678B1537CC1828D93580C9E5849A9653175ACB753F2BE7437BE45F6C603E485F2EC301BB42B6C37C225D7495A584AE231890AB5C8C35C268CF4BBB0213C096019319561A8A6947637AA40D006B415BB2CFA2237E0890B6A3BC134ABF8F6585E108D15940F91F4BF5B0C818055B21DEA6E63B553988C47F4B94E7CF800A493B4734705EDC56A4B6021C629500675876804CF0B951F038A5C7FE58E89774EF2992FD7C63099D352A7D21560B788B405709861817E59A96B3A3A83CBA803B16934331071905BBEC6532900155D8AC88CB32E4E21A3BD3A03FDEC325A51CD2773964E6784FCF1853737AA64EB67564727272661ABF84313A57A44B123C65509CFB7A6F6641CDCC3B57FE628C7B8192DB44FFBF5796A8613B1FA126F6076883C783DC24E2A4464C40B3A41CA70AE87620866CF4FCB2BD204BF5C283812BA056AC0C345E379C4BA24D750901279BB2F3A16F612BFADB35703332C7C136F68EAB6755C66B6A4AD1AABA7B768A58ACAACC10A459A1CC8EF29377BC200E4D315A30A6BCC3256F9734D06E9779CAA5442A9A16069081377C76E75154368072DC446ED6C8B8E622A21E383CF9BA1FB434E2ECC81E7B78CEE986B8FF798AB18CF9634543546284EDA2A26B47F05B735BCDB1202220076DC8B4E4B9F853533C8F6C7FF38817BA49712835785F17F14CA01D0C1C1E98810FE0B36E5B427157B9418449CEDD641A4293C85C32700102ACEC22EBAD98ED160A5F027BD4CDA57F1F3720A12C134654DD5E73F829676495390D0E7929D6034E9C55F7D55BA658BC587988E8AF94960F6CFB8D5AF7A0021535A6E25E437D49A780698BE22AC9953949F571B85A685725F8207A2B0AE849B601AB91B159B3DF4A154C2041E776070AFC42969322380917C97510799F3149131477E16663D3174C7C1CAEA788535C6C005A64F2868631B31B66E205FD38C1D84542D0F1B578F58C9BF5A0FAEAB6AB6494893053165EAFD465FC64A0C5F8F3F9003489415899D59A543D8208C54A3166529B539228A39E87D531F3527C207EDCC1DB7FADDCF9628391879B335C707839A0DB051A88626ED79D451140800E03B59B956F8210E556067407D13DC90FA9E8B872BFB8F
            ct = A6AF29D5F5B80BD130F518BADDD6C8F17545413D860FB3DE451979EBFA5E4E3112C7C0ADF99824BB526F2C3550748ED0E134F0457A7C61F9F526F002BAADC03FC13E38131219513C3EDE061661E74F603C4FCF7951C8E52C9C213B0D22D9293663D669A6B58ED8FCEFCF8249D7BB5298F55761445B2B83CE7F005CB04248AEC8BDA22FD2D42AA766322014EA038CC32C55C8E4B9E28EC9119F527341E4F66A035121073B85DE6706DA19E0838A9F33B719A68F039B664DC002659EABFC398679AA7009CE0CD01CDAFB6CD2A26FE4101672C98FF58F7C47D5BDA2906653B3A6F9651F7A121EA77EA74723FAE5B873F9BB7B664F0C8A93831EF9D51C7CC1EF44AC0E55A55CA76D137FE9B75F40509CEF156E5AD18F9FB999680008E547D55EECD5B4D1CB1D9F076CEC21501C7402509ECB77AFB2CB9A61340A8BD1514C6E71B4AA45E47EC37512271B911F8FB46C9082C9DF07204ABB5A50E6E3647A8AD4D8D5D7BFF19C8A509308BCFB895536D045CA2B97CB16A29BB7181CAD0509DDB91735028EBA8C31D74BD275EAA65B5340B3A43FBFE0B3061D6BAE7E75B7098CDABE91D4B31E36C9AA7A8298862AD63C8FD282E03B460B3AB464CE0F27B1C3D11155ACAA011EB9E2AE3E6DDA07D6F491737CBCE9B05F9BC56BE20E8D326BA132C57FB235161144519CDF40560FBE279BDE411E112531F826D6AB10D4547350ADD2A9DE8D62C2AC82CABE6815646F4DC9742BB0C2A3F77EC7B46C6B537605FA31798CD89281221A33DFB9796E644305630332C2CB931408AB481A16D953F6BEAE3891D6D9AC1FAB38222D9271872D9D0CADB91ABE9B4E265F75C6E5E829E146C3D8CE1E9D12E0D129801957F46B0D2DBE1F749B1D08E2345F6239A731342EB75B0CF1BF411749BC2CAF2810B788C6B7238B4D3DA2D6315CE9542E24404F145755A30AB851E4445841BD33F716A586884888ECC6BC6498AA32919AE81D20C26973C2BD54582A0F6AD98ABFD2627E15690A727E69F581DD2A7127982A90E33E2D4A03FE339142C7E44C326AC46ED395A225D3033389917328B45316B1585A01B2C304B2944E903ABBB3EC5619441CFC8965A446DF75DEFA80C6E15ADBD506B7AB2DE12DDA9BC81441CFC89052E2E5808F7126C6FD3AC6AC8081258A84A09AE50F6CD7CC0F4AF336FD1D643E99079996268C2D32D909F22E3504F07FBB563196D4312FDDB9335D5C1D36E8C5EEA2278DBA23B94D193C947CC41CA993DC7DB1396340AD9C4FE687DD7B8D0C7A5120AE0204F2C665BD5F473D644C7FF26BFFBA7A36980830702128A7E661D677A092A36E7428A4139FB29B0095CC11086F447D2A9EF6C9B161F189C6299E084CB7AA00FAF787797BFB069FBC087FDE26252A1664F19C5A8A22EC5EE1AEB076357B7DC37E6B0F1520F958F7851BACB92C89FD114A72FEAC54652D45B09E1AE7651ABD164BCD537D58FA39D3EC8ACDCDF98425005862FA59692DE162B77E6297C66233348408A8AB695CE2F2728DB9FBE27E958967EC5974767C5A66023074B4A71AFD264AD2890E970A1F31D6E3311B736F9F9488793DDC88F23458064254C82A1D9E59EAD2FCEC40B430687C4B7E28960926AFCACC9BD756A71088C78450E20A2E980AEDE9EBEDFE7FABD6ABFE96F934C4B02C01CA194D01B73C25D5997039D3FCD0F099521F70CAEE69110AC1FC5A99917AD752FC96ADFAD7186D0A7C9CFE5601C07514EA6448D661C57AA20242103C4276A070A489A4CB6BCA0F9ECC4379FB220215FD91F81019D5B0AE619358B52468F272C178E3A74CF6775AA924FE329C3175D9E4C3E21AB9EC836EDC3ACAB2E3891EE8DEDA515D39AF9B8DDD0EE7B0164F805C3835F6D2BABDB30EAB4756E7EC7F829ECE01E8EADFBBED12FC283B3D4C69F575E7F80417689FDFCFC7BE27EE3B8CDF57AAEBEC4A95B7E5BB585B85227F7C32BE30DB3E65E42E30DCF5A5FA073DBA399D942F2222ADB9B9898102AFE5432EDC7F04AE34A8FEC2D81CB49A9A9B43814CE71D97F726E2B1E8F64B50E65DFB4816E12E82A3197484A4E9BBA4D2D69E3F19D0B75C21E2BFFE9FC0C98CF48A3AAF08D467F72687DF0178174B7897F734349B181ECA86A598A0C5E8C25946F24DC5572BD324A40458A788E5137F3C7A7C97FC9F12A3C463A8FE9449101CCE966D7C009323932998D56EF430C73BC24F5D95F737858DDC4F32C013
            ss = B10F7394926AD3B49C5D62D5AEB531D5757538BCC0DA9E550D438F1B61BD7419
            */
            string temp = "061550234D158C5EC95595FE04EF7A25767F2E24CC2BC479D09D86DC9ABCFDE7056A8C266F9EF97ED08541DBD2E1FFA1";
            string expectedPubKey = "D22302CBD3399FACC630991FC8F28BDB4354762541527678BCF61F65C241146C426D23B9BFAA6B7DF18C97F20C1B6125BF874B1D89475852C448215DB0EB7737F91480E8CEBD9A0871574F5AB62D9020175EC6927CA0B54C09818E42CF92A383172422C7DC1831D63B0C295DE75159DB8034E9E07F7B0B910C3C1E5FB66B3DC523F1FA6EB4910CB89A6C17562C83AB4C18D0CD7E0796592A372AA409B1C557347CCACDC4644A119064D06DD474929D1C6FB4D686E5491CE4BC89A30BB4B8C41BCE5157DFC1360823B1AB618C14B10F98C25067398EA7018C278A4B3DF31334D603B2044EF187CD9BC6CE42725BD962C264983E9E18155A8B9C47143D70460A26A56FE7658C1F150348C6087EF758AD167887860A007A5FC37358D43B5EBEE820ACEA474F0AC07B76802866199C61231D5C747C93774D2C1E0C1C67E6C81B82752173E125BAF39B4FD19A4F453DC57976B1D97FE6996992BBB65B7CB25D077BBAA6A13322899AF659CF1B3558C1B5001154B625809ED89AEEBB89E6EA7D67F723D045AB05715C42355DA6A5C8DD39C8ABE3037751A01ED1C7374919F3121B5A52C53D1487316769F80721DEEAAAD3C90F76E7AE9E12BA92B32B5FD457E3C752C2650DFB885771CB77AC3C785A8C562E6A1C63C2A55EA47CF8B90EB8225C123C346452566235B2F31823A33521E087937A345D8D663EEAA05658917BBAA008C2E335F8850A90A326D0E66432F44CEB8289E4ECB2D12958E984072ECACB88E1348FF0B55654ACBA5B54971CBAEBA88EC4B91A94C37192FA982BECB9F3DA421603B61A51BC8E36CBD053851C77B1B926B17A272AA9023246B02B3ED47F66A00BD5684823634E7CE58CF8F306E35B1E5322824D904801F0A2FA7C2BC9C252B0A56B7BA2AB0F636021745A70A9A43E2B0A8D615970B65309624B5184BCC30B911679AEDD76025FE3908FD67897B0CF4BE5A6F5413D7DD98564B23E42A93E4AA8821CD45054C643EDC1158DB6B3DEB13FB5A51EBD1A8A78B87225A7338E101104C4A220D9BDEDD48C85A1C2DAE781A80C40E13B87EAC73A764201C9B760CCFB1AE392699C7039D27C39362B27B8FC6F07A8A3D4410F1547C48A9997F62C61074452EF1515F8A649EBCA9437205A4E8A61606B41DAF6834D671F4D852C0C9C4096611648C6A3170678B1537CC1828D93580C9E5849A9653175ACB753F2BE7437BE45F6C603E485F2EC301BB42B6C37C225D7495A584AE231890AB5C8C35C268CF4BBB0213C096019319561A8A6947637AA40D006B415BB2CFA2237E0890B6A3BC134ABF8F6585E108D15940F91F4BF5B0C818055B21DEA6E63B553988C47F4B94E7CF800A493B4734705EDC56A4B6021C629500675876804CF0B951F038A5C7FE58E89774EF2992FD7C63099D352A7D21560B788B405709861817E59A96B3A3A83CBA803B16934331071905BBEC6532900155D8AC88CB32E4E21A3BD3A03FDEC325A51CD2773964E6784FCF1853737AA64EB67564727272661ABF84313A57A44B123C65509CFB7A6F6641CDCC3B57FE628C7B8192DB44FFBF5796A8613B1FA126F6076883C783DC24E2A4464C40B3A41CA70AE87620866CF4FCB2BD204BF5C283812BA056AC0C345E379C4BA24D750901279BB2F3A16F612BFADB35703332C7C136F68EAB6755C66B6A4AD1AABA7B768A58ACAACC10A459A1CC8EF29377BC200E4D315A30A6BCC3256F9734D06E9779CAA5442A9A16069081377C76E75154368072DC446ED6C8B8E622A21E383CF9BA1FB434E2ECC81E7B78CEE986B8FF798AB18CF9634543546284EDA2A26B47F05B735BCDB1202220076DC8B4E4B9F853533C8F6C7FF38817BA49712835785F17F14CA01D0C1C1E98810FE0B36E5B427157B9418449CEDD641A4293C85C32700102ACEC22EBAD98ED160A5F027BD4CDA57F1F3720A12C134654DD5E73F829676495390D0E7929D6034E9C55F7D55BA658BC587988E8AF94960F6CFB8D5AF7A0021535A6E25E437D49A780698BE22AC9953949F571B85A685725F8207A2B0AE849B601AB91B159B3DF4A154C2041E776070AFC42969322380917C97510799F3149131477E16663D3174C7C1CAEA788535C6C005A64F2868631B31B66E205FD38C1D84542D0F1B578F58C9BF5A0FAEAB6AB6494893053165EAFD465FC64A0C5F8F3F9003489415899D59A543D8208C54A3166529B53922";
            string expectedPrivKey = "07638FB69868F3D320E5862BD96933FEB311B362093C9B5D50170BCED43F1B536D9A204BB1F22695950BA1F2A9E8EB828B284488760B3FC84FABA04275D5628E39C5B2471374283C503299C0AB49B66B8BBB56A4186624F919A2BA59BB08D8551880C2BEFC4F87F25F59AB587A79C327D792D54C974A69262FF8A78938289E9A87B688B083E0595FE218B6BB1505941CE2E81A5A64C5AAC60417256985349EE47A52420A5F97477B7236AC76BC70E8288729287EE3E34A3DBC3683C0B7B10029FC203418537E7466BA6385A8FF301EE12708F82AAA1E380FC7A88F8F205AB7E88D7E95952A55BA20D09B79A47141D62BF6EB7DD307B08ECA13A5BC5F6B68581C6865B27BBCDDAB142F4B2CBFF488C8A22705FAA98A2B9EEA3530C76662335CC7EA3A00777725EBCCCD2A4636B2D9122FF3AB77123CE0883C1911115E50C9E8A94194E48DD0D09CFFB3ADCD2C1E92430903D07ADBF00532031575AA7F9E7B5A1F3362DEC936D4043C05F2476C07578BC9CBAF2AB4E382727AD41686A96B2548820BB03B32F11B2811AD62F489E951632ABA0D1DF89680CC8A8B53B481D92A68D70B4EA1C3A6A561C0692882B5CA8CC942A8D495AFCB06DE89498FB935B775908FE7A03E324D54CC19D4E1AABD3593B38B19EE1388FE492B43127E5A504253786A0D69AD32601C28E2C88504A5BA599706023A61363E17C6B9BB59BDC697452CD059451983D738CA3FD034E3F5988854CA05031DB09611498988197C6B30D258DFE26265541C89A4B31D6864E9389B03CB74F7EC4323FB9421A4B9790A26D17B0398A26767350909F84D57B6694DF830664CA8B3C3C03ED2AE67B89006868A68527CCD666459AB7F056671000C6164D3A7F266A14D97CBD7004D6C92CACA770B844A4FA9B182E7B18CA885082AC5646FCB4A14E1685FEB0C9CE3372AB95365C04FD83084F80A23FF10A05BF15F7FA5ACC6C0CB462C33CA524FA6B8BB359043BA68609EAA2536E81D08463B19653B5435BA946C9ADDEB202B04B031CC960DCC12E4518D428B32B257A4FC7313D3A7980D80082E934F9D95C32B0A0191A23604384DD9E079BBBAA266D14C3F756B9F2133107433A4E83FA7187282A809203A4FAF841851833D121AC383843A5E55BC2381425E16C7DB4CC9AB5C1B0D91A47E2B8DE0E582C86B6B0D907BB360B97F40AB5D038F6B75C814B27D9B968D419832BC8C2BEE605EF6E5059D33100D90485D378450014221736C07407CAC260408AA64926619788B8601C2A752D1A6CBF820D7C7A04716203225B3895B9342D147A8185CFC1BB65BA06B4142339903C0AC4651385B45D98A8B19D28CD6BAB088787F7EE1B12461766B43CBCCB96434427D93C065550688F6948ED1B5475A425F1B85209D061C08B56C1CC069F6C0A7C6F29358CAB911087732A649D27C9B98F9A48879387D9B00C25959A71654D6F6A946164513E47A75D005986C2363C09F6B537ECA78B9303A5FA457608A586A653A347DB04DFCC19175B3A301172536062A658A95277570C8852CA8973F4AE123A334047DD711C8927A634A03388A527B034BF7A8170FA702C1F7C23EC32D18A2374890BE9C787A9409C82D192C4BB705A2F996CE405D85A4C1A1AB9B6AEB49CCE1C2F8A97C3516C72A00A46263BAA696BF25727719C3216423618FF33380934A6C10545C4C5C5155B12486181FC7A2319873978B6A2A67490F8256BD2196FE1792A4C00077B812EAE8BED3572499684AB3371876761E450C9F9D2768A36806D7AB2046C91F17599E9AC592990808DCD7B4D0919072F14EC361773B7252444C323C308326F4A30F8680D2F748F56A132B82674ED0184620B82AD2CB182C97B481626647491290A011CC73828685A8C367A5B9CF8D621B0D5C1EFF03172758BD004978C251CD51342228989CAE6332AC486437CB5C57D4307462865253BE217B3515C73DF405B7F28217AD0B8CF60C2FFFAA0A0048B1FB4ACDCDC38B5250CFEC356A6DE26CFA7A588FDC86F98C854AC64C7BFAA96F5A32CC0610934BAA6A586B9A2054F13BA274174AA0D2B3A81B96A940666F789B5A6BCDC0A6A0178A0C9A02578A493F6EEA0D2E6C13951C9F249A5E8DD71DD49A742D451F1ABBA19AF8C547855E0AFC728E90ABB499C9BEEB766F4729CDA22263E324D22302CBD3399FACC630991FC8F28BDB4354762541527678BCF61F65C241146C426D23B9BFAA6B7DF18C97F20C1B6125BF874B1D89475852C448215DB0EB7737F91480E8CEBD9A0871574F5AB62D9020175EC6927CA0B54C09818E42CF92A383172422C7DC1831D63B0C295DE75159DB8034E9E07F7B0B910C3C1E5FB66B3DC523F1FA6EB4910CB89A6C17562C83AB4C18D0CD7E0796592A372AA409B1C557347CCACDC4644A119064D06DD474929D1C6FB4D686E5491CE4BC89A30BB4B8C41BCE5157DFC1360823B1AB618C14B10F98C25067398EA7018C278A4B3DF31334D603B2044EF187CD9BC6CE42725BD962C264983E9E18155A8B9C47143D70460A26A56FE7658C1F150348C6087EF758AD167887860A007A5FC37358D43B5EBEE820ACEA474F0AC07B76802866199C61231D5C747C93774D2C1E0C1C67E6C81B82752173E125BAF39B4FD19A4F453DC57976B1D97FE6996992BBB65B7CB25D077BBAA6A13322899AF659CF1B3558C1B5001154B625809ED89AEEBB89E6EA7D67F723D045AB05715C42355DA6A5C8DD39C8ABE3037751A01ED1C7374919F3121B5A52C53D1487316769F80721DEEAAAD3C90F76E7AE9E12BA92B32B5FD457E3C752C2650DFB885771CB77AC3C785A8C562E6A1C63C2A55EA47CF8B90EB8225C123C346452566235B2F31823A33521E087937A345D8D663EEAA05658917BBAA008C2E335F8850A90A326D0E66432F44CEB8289E4ECB2D12958E984072ECACB88E1348FF0B55654ACBA5B54971CBAEBA88EC4B91A94C37192FA982BECB9F3DA421603B61A51BC8E36CBD053851C77B1B926B17A272AA9023246B02B3ED47F66A00BD5684823634E7CE58CF8F306E35B1E5322824D904801F0A2FA7C2BC9C252B0A56B7BA2AB0F636021745A70A9A43E2B0A8D615970B65309624B5184BCC30B911679AEDD76025FE3908FD67897B0CF4BE5A6F5413D7DD98564B23E42A93E4AA8821CD45054C643EDC1158DB6B3DEB13FB5A51EBD1A8A78B87225A7338E101104C4A220D9BDEDD48C85A1C2DAE781A80C40E13B87EAC73A764201C9B760CCFB1AE392699C7039D27C39362B27B8FC6F07A8A3D4410F1547C48A9997F62C61074452EF1515F8A649EBCA9437205A4E8A61606B41DAF6834D671F4D852C0C9C4096611648C6A3170678B1537CC1828D93580C9E5849A9653175ACB753F2BE7437BE45F6C603E485F2EC301BB42B6C37C225D7495A584AE231890AB5C8C35C268CF4BBB0213C096019319561A8A6947637AA40D006B415BB2CFA2237E0890B6A3BC134ABF8F6585E108D15940F91F4BF5B0C818055B21DEA6E63B553988C47F4B94E7CF800A493B4734705EDC56A4B6021C629500675876804CF0B951F038A5C7FE58E89774EF2992FD7C63099D352A7D21560B788B405709861817E59A96B3A3A83CBA803B16934331071905BBEC6532900155D8AC88CB32E4E21A3BD3A03FDEC325A51CD2773964E6784FCF1853737AA64EB67564727272661ABF84313A57A44B123C65509CFB7A6F6641CDCC3B57FE628C7B8192DB44FFBF5796A8613B1FA126F6076883C783DC24E2A4464C40B3A41CA70AE87620866CF4FCB2BD204BF5C283812BA056AC0C345E379C4BA24D750901279BB2F3A16F612BFADB35703332C7C136F68EAB6755C66B6A4AD1AABA7B768A58ACAACC10A459A1CC8EF29377BC200E4D315A30A6BCC3256F9734D06E9779CAA5442A9A16069081377C76E75154368072DC446ED6C8B8E622A21E383CF9BA1FB434E2ECC81E7B78CEE986B8FF798AB18CF9634543546284EDA2A26B47F05B735BCDB1202220076DC8B4E4B9F853533C8F6C7FF38817BA49712835785F17F14CA01D0C1C1E98810FE0B36E5B427157B9418449CEDD641A4293C85C32700102ACEC22EBAD98ED160A5F027BD4CDA57F1F3720A12C134654DD5E73F829676495390D0E7929D6034E9C55F7D55BA658BC587988E8AF94960F6CFB8D5AF7A0021535A6E25E437D49A780698BE22AC9953949F571B85A685725F8207A2B0AE849B601AB91B159B3DF4A154C2041E776070AFC42969322380917C97510799F3149131477E16663D3174C7C1CAEA788535C6C005A64F2868631B31B66E205FD38C1D84542D0F1B578F58C9BF5A0FAEAB6AB6494893053165EAFD465FC64A0C5F8F3F9003489415899D59A543D8208C54A3166529B539228A39E87D531F3527C207EDCC1DB7FADDCF9628391879B335C707839A0DB051A88626ED79D451140800E03B59B956F8210E556067407D13DC90FA9E8B872BFB8F";

            byte[] seed = Hex.Decode(temp);

            NistSecureRandom random = new NistSecureRandom(seed, null);

            KyberKeyPairGenerator keyGen = new KyberKeyPairGenerator();
            keyGen.Init(new KyberKeyGenerationParameters(random, KyberParameters.kyber1024));

            AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();
            Assert.IsTrue(Arrays.AreEqual(Hex.Decode(expectedPubKey), ((KyberPublicKeyParameters)keyPair.Public).GetEncoded()));

            Assert.IsTrue(Arrays.AreEqual(Hex.Decode(expectedPrivKey), ((KyberPrivateKeyParameters)keyPair.Private).GetEncoded()));

            KyberKemGenerator kemGen = new KyberKemGenerator(random);

            ISecretWithEncapsulation secretEncap = kemGen.GenerateEncapsulated(keyPair.Public);

            string expectedSharedSecret = "B10F7394926AD3B49C5D62D5AEB531D5757538BCC0DA9E550D438F1B61BD7419";

            Assert.IsTrue(Arrays.AreEqual(Hex.Decode(expectedSharedSecret), secretEncap.GetSecret()));

            string expectedCipherText = "A6AF29D5F5B80BD130F518BADDD6C8F17545413D860FB3DE451979EBFA5E4E3112C7C0ADF99824BB526F2C3550748ED0E134F0457A7C61F9F526F002BAADC03FC13E38131219513C3EDE061661E74F603C4FCF7951C8E52C9C213B0D22D9293663D669A6B58ED8FCEFCF8249D7BB5298F55761445B2B83CE7F005CB04248AEC8BDA22FD2D42AA766322014EA038CC32C55C8E4B9E28EC9119F527341E4F66A035121073B85DE6706DA19E0838A9F33B719A68F039B664DC002659EABFC398679AA7009CE0CD01CDAFB6CD2A26FE4101672C98FF58F7C47D5BDA2906653B3A6F9651F7A121EA77EA74723FAE5B873F9BB7B664F0C8A93831EF9D51C7CC1EF44AC0E55A55CA76D137FE9B75F40509CEF156E5AD18F9FB999680008E547D55EECD5B4D1CB1D9F076CEC21501C7402509ECB77AFB2CB9A61340A8BD1514C6E71B4AA45E47EC37512271B911F8FB46C9082C9DF07204ABB5A50E6E3647A8AD4D8D5D7BFF19C8A509308BCFB895536D045CA2B97CB16A29BB7181CAD0509DDB91735028EBA8C31D74BD275EAA65B5340B3A43FBFE0B3061D6BAE7E75B7098CDABE91D4B31E36C9AA7A8298862AD63C8FD282E03B460B3AB464CE0F27B1C3D11155ACAA011EB9E2AE3E6DDA07D6F491737CBCE9B05F9BC56BE20E8D326BA132C57FB235161144519CDF40560FBE279BDE411E112531F826D6AB10D4547350ADD2A9DE8D62C2AC82CABE6815646F4DC9742BB0C2A3F77EC7B46C6B537605FA31798CD89281221A33DFB9796E644305630332C2CB931408AB481A16D953F6BEAE3891D6D9AC1FAB38222D9271872D9D0CADB91ABE9B4E265F75C6E5E829E146C3D8CE1E9D12E0D129801957F46B0D2DBE1F749B1D08E2345F6239A731342EB75B0CF1BF411749BC2CAF2810B788C6B7238B4D3DA2D6315CE9542E24404F145755A30AB851E4445841BD33F716A586884888ECC6BC6498AA32919AE81D20C26973C2BD54582A0F6AD98ABFD2627E15690A727E69F581DD2A7127982A90E33E2D4A03FE339142C7E44C326AC46ED395A225D3033389917328B45316B1585A01B2C304B2944E903ABBB3EC5619441CFC8965A446DF75DEFA80C6E15ADBD506B7AB2DE12DDA9BC81441CFC89052E2E5808F7126C6FD3AC6AC8081258A84A09AE50F6CD7CC0F4AF336FD1D643E99079996268C2D32D909F22E3504F07FBB563196D4312FDDB9335D5C1D36E8C5EEA2278DBA23B94D193C947CC41CA993DC7DB1396340AD9C4FE687DD7B8D0C7A5120AE0204F2C665BD5F473D644C7FF26BFFBA7A36980830702128A7E661D677A092A36E7428A4139FB29B0095CC11086F447D2A9EF6C9B161F189C6299E084CB7AA00FAF787797BFB069FBC087FDE26252A1664F19C5A8A22EC5EE1AEB076357B7DC37E6B0F1520F958F7851BACB92C89FD114A72FEAC54652D45B09E1AE7651ABD164BCD537D58FA39D3EC8ACDCDF98425005862FA59692DE162B77E6297C66233348408A8AB695CE2F2728DB9FBE27E958967EC5974767C5A66023074B4A71AFD264AD2890E970A1F31D6E3311B736F9F9488793DDC88F23458064254C82A1D9E59EAD2FCEC40B430687C4B7E28960926AFCACC9BD756A71088C78450E20A2E980AEDE9EBEDFE7FABD6ABFE96F934C4B02C01CA194D01B73C25D5997039D3FCD0F099521F70CAEE69110AC1FC5A99917AD752FC96ADFAD7186D0A7C9CFE5601C07514EA6448D661C57AA20242103C4276A070A489A4CB6BCA0F9ECC4379FB220215FD91F81019D5B0AE619358B52468F272C178E3A74CF6775AA924FE329C3175D9E4C3E21AB9EC836EDC3ACAB2E3891EE8DEDA515D39AF9B8DDD0EE7B0164F805C3835F6D2BABDB30EAB4756E7EC7F829ECE01E8EADFBBED12FC283B3D4C69F575E7F80417689FDFCFC7BE27EE3B8CDF57AAEBEC4A95B7E5BB585B85227F7C32BE30DB3E65E42E30DCF5A5FA073DBA399D942F2222ADB9B9898102AFE5432EDC7F04AE34A8FEC2D81CB49A9A9B43814CE71D97F726E2B1E8F64B50E65DFB4816E12E82A3197484A4E9BBA4D2D69E3F19D0B75C21E2BFFE9FC0C98CF48A3AAF08D467F72687DF0178174B7897F734349B181ECA86A598A0C5E8C25946F24DC5572BD324A40458A788E5137F3C7A7C97FC9F12A3C463A8FE9449101CCE966D7C009323932998D56EF430C73BC24F5D95F737858DDC4F32C013";

            Assert.IsTrue(Arrays.AreEqual(Hex.Decode(expectedCipherText), secretEncap.GetEncapsulation()));

            KyberKemExtractor kemExtract = new KyberKemExtractor((KyberPrivateKeyParameters)keyPair.Private);

            byte[] decryptedSharedSecret = kemExtract.ExtractSecret(secretEncap.GetEncapsulation());

            Assert.IsTrue(Arrays.AreEqual(Hex.Decode(expectedSharedSecret), decryptedSharedSecret));
        }

        [Test]
        public void TestKyberRandom()
        {
            SecureRandom random = new SecureRandom();
            KyberKeyPairGenerator keyGen = new KyberKeyPairGenerator();

            keyGen.Init(new KyberKeyGenerationParameters(random, KyberParameters.kyber1024));

            for (int i = 0; i != 1000; i++)
            {
                AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();

                KyberKemGenerator kemGen = new KyberKemGenerator(random);

                ISecretWithEncapsulation secretEncap = kemGen.GenerateEncapsulated(keyPair.Public);

                KyberKemExtractor kemExtract = new KyberKemExtractor((KyberPrivateKeyParameters)keyPair.Private);

                byte[] decryptedSharedSecret = kemExtract.ExtractSecret(secretEncap.GetEncapsulation());

                Assert.IsTrue(Arrays.AreEqual(secretEncap.GetSecret(), decryptedSharedSecret));
            }
        }

        [Test]
        public void TestParameters()
        {
            Assert.AreEqual(128, KyberParameters.kyber512.SessionKeySize);
            Assert.AreEqual(192, KyberParameters.kyber768.SessionKeySize);
            Assert.AreEqual(256, KyberParameters.kyber1024.SessionKeySize);
        }

        [Test]
        public void TestPrivInfoGeneration()
        {
            SecureRandom random = new SecureRandom();
            PqcOtherInfoGenerator.PartyU partyU = new PqcOtherInfoGenerator.PartyU(KyberParameters.kyber512,
                new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1), Hex.Decode("beef"), Hex.Decode("cafe"), random);

            byte[] partA = partyU.GetSuppPrivInfoPartA();

            PqcOtherInfoGenerator.PartyV partyV = new PqcOtherInfoGenerator.PartyV(KyberParameters.kyber512,
                new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1), Hex.Decode("beef"), Hex.Decode("cafe"), random);

            byte[] partB = partyV.GetSuppPrivInfoPartB(partA);

            DerOtherInfo otherInfoU = partyU.Generate(partB);

            DerOtherInfo otherInfoV = partyV.Generate();

            Assert.IsTrue(Arrays.AreEqual(otherInfoU.GetEncoded(), otherInfoV.GetEncoded()));
        }

        [Test]
        public void TestRng()
        {
            string temp = "061550234D158C5EC95595FE04EF7A25767F2E24CC2BC479D09D86DC9ABCFDE7056A8C266F9EF97ED08541DBD2E1FFA1";
            byte[] seed = Hex.Decode(temp);
            NistSecureRandom r = new NistSecureRandom(seed, null);
            byte[] testBytes = new byte[48];
            r.NextBytes(testBytes);

            string randBytesString = "7C9935A0B07694AA0C6D10E4DB6B1ADD2FD81A25CCB148032DCD739936737F2DB505D7CFAD1B497499323C8686325E47";
            byte[] randBytes = Hex.Decode(randBytesString);

            Assert.IsTrue(Arrays.AreEqual(randBytes, testBytes));
        }

        [TestCaseSource(nameof(TestVectorFiles))]
        [Parallelizable(ParallelScope.All)]
        public void TV(string testVectorFile)
        {
            RunTestVectorFile(testVectorFile);
        }

        [TestCaseSource(nameof(TestVectorFilesAes))]
        [Parallelizable(ParallelScope.All)]
        public void TVAes(string testVectorFile)
        {
            RunTestVectorFile(testVectorFile);
        }

        private static void RunTestVector(string name, IDictionary<string, string> buf)
        {
            string count = buf["count"];
            byte[] seed = Hex.Decode(buf["seed"]); // seed for SecureRandom
            byte[] pk = Hex.Decode(buf["pk"]); // public key
            byte[] sk = Hex.Decode(buf["sk"]); // private key
            byte[] ct = Hex.Decode(buf["ct"]); // ciphertext
            byte[] ss = Hex.Decode(buf["ss"]); // session key

            NistSecureRandom random = new NistSecureRandom(seed, null);
            KyberParameters kyberparameters = Parameters[name];

            KyberKeyPairGenerator kpGen = new KyberKeyPairGenerator();
            KyberKeyGenerationParameters genParam = new KyberKeyGenerationParameters(random, kyberparameters);
            
            //
            // Generate keys and test.
            //
            kpGen.Init(genParam);
            AsymmetricCipherKeyPair kp = kpGen.GenerateKeyPair();

            KyberPublicKeyParameters pubParams = (KyberPublicKeyParameters)PqcPublicKeyFactory.CreateKey(
                PqcSubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo((KyberPublicKeyParameters)kp.Public));
            KyberPrivateKeyParameters privParams = (KyberPrivateKeyParameters)PqcPrivateKeyFactory.CreateKey(
                PqcPrivateKeyInfoFactory.CreatePrivateKeyInfo((KyberPrivateKeyParameters)kp.Private));

            Assert.True(Arrays.AreEqual(pk, pubParams.GetEncoded()), name + " " + count + ": public key");
            Assert.True(Arrays.AreEqual(sk, privParams.GetEncoded()), name + " " + count + ": secret key");

            // KEM Enc
            KyberKemGenerator KyberEncCipher = new KyberKemGenerator(random);
            ISecretWithEncapsulation secWenc = KyberEncCipher.GenerateEncapsulated(pubParams);
            byte[] generated_cipher_text = secWenc.GetEncapsulation();
            Assert.True(Arrays.AreEqual(ct, generated_cipher_text), name + " " + count + ": kem_enc cipher text");
            byte[] secret = secWenc.GetSecret();
            Assert.True(Arrays.AreEqual(ss, secret), name + " " + count + ": kem_enc key");

            // KEM Dec
            KyberKemExtractor KyberDecCipher = new KyberKemExtractor(privParams);

            byte[] dec_key = KyberDecCipher.ExtractSecret(generated_cipher_text);

            Assert.True(Arrays.AreEqual(dec_key, ss), name + " " + count + ": kem_dec ss");
            Assert.True(Arrays.AreEqual(dec_key, secret), name + " " + count + ": kem_dec key");
        }

        public static void RunTestVectorFile(string name)
        {
            var buf = new Dictionary<string, string>();
            TestSampler sampler = new TestSampler();
            using (var src = new StreamReader(SimpleTest.GetTestDataAsStream("pqc.crystals.kyber." + name)))
            {
                string line;
                while ((line = src.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.StartsWith("#"))
                        continue;

                    if (line.Length > 0)
                    {
                        int a = line.IndexOf('=');
                        if (a > -1)
                        {
                            buf[line.Substring(0, a).Trim()] = line.Substring(a + 1).Trim();
                        }
                        continue;
                    }

                    if (buf.Count > 0)
                    {
                        if (!sampler.SkipTest(buf["count"]))
                        {
                            RunTestVector(name, buf);
                        }
                        buf.Clear();
                    }
                }

                if (buf.Count > 0)
                {
                    if (!sampler.SkipTest(buf["count"]))
                    {
                        RunTestVector(name, buf);
                    }
                    buf.Clear();
                }
            }
        }
    }
}
