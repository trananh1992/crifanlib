/*
 * [File]
 * crifanLibAmazon.cs
 * 
 * [Function]
 * Crifan Lib of C# version for Amazon
 * 
 * [Note]
 * 1.use crifanLib.cs
 * http://www.crifan.com/crifan_released_all/crifanlib/
 * http://www.crifan.com/crifan_csharp_lib_crifanlib_cs/
 * 2.use HtmlAgilityPack
 *  
 * [Version]
 * v1.0
 * 
 * [update]
 * 2013-06-01
 * 
 * [Author]
 * Crifan Li
 * 
 * [Contact]
 * http://www.crifan.com/contact_me/
 * 
 * [History]
 * [v1.0]
 * 1. initial added 
 * extractMainCategoryList
 * extractNextPageUrl
 * extractProductTitle
 * extractProductBulletList
 * extractProductDescription
 * extractProductImageList
 * extractSearchItemList
 * extractProductBuyerNumberAndNewUrl
 */

using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;

using System.Web;
using HtmlAgilityPack;

public class crifanLibAmazon
{
    public static string constAmazonDomainUrl = "http://www.amazon.com";

    public crifanLib crl;
    public crifanLibAmazon()
    {
        //init something
        crl = new crifanLib();
    }

    public struct categoryItem
    {
        public string Name { get; set; } //Amazon Instant Video
        //public string Url { get; set; } //http://www.amazon.com/s/ref=nb_sb_noss?url=search-alias%3dinstant-video
        public string Key { get; set; } //instant-video
        //public string name;
        //public string url;
    };

    public struct searchResultItem
    {
        public string productUrl;
        //add more if need
    }

    /*
     * [Function]
     * extract next page url
     * [Input]
     * current page html of 
     * http://www.amazon.com/s/ref=nb_sb_noss?url=search-alias%3dinstant-video
     * [Output]
     * http://www.amazon.com/s/ref=nb_sb_noss?url=search-alias%3Dinstant-video#/ref=sr_pg_2?rh=n%3A2858778011&page=2&ie=UTF8&qid=1368688123
     * [Note]
     */
    public bool extractNextPageUrl(string curPageHtml, out string nextPageUrl)
    {
        bool gotNextPageUrl = false;
        nextPageUrl = "";

        /*
            <a title="Next Page" 
                        id="pagnNextLink" 
                        class="pagnNext" 
                        href="/s/ref=sr_pg_2?rh=n%3A2858778011&amp;page=2&amp;ie=UTF8&amp;qid=1368696683">
                        <span id="pagnNextString">Next Page</span>
                        <span class="srSprite pagnNextArrow"></span>
                        </a>
            */
        HtmlAgilityPack.HtmlDocument htmlDoc = crl.htmlToHtmlDoc(curPageHtml);
        HtmlNode nextPageNode = htmlDoc.DocumentNode.SelectSingleNode("//a[@id='pagnNextLink']");
        if (nextPageNode != null)
        {
            string hrefValue = nextPageNode.Attributes["href"].Value;//"/s/ref=sr_pg_2?rh=n%3A2625373011%2Cn%3A%212644981011%2Cn%3A%212644982011%2Cn%3A2858778011&amp;page=2&amp;ie=UTF8&amp;qid=1368697688"
            nextPageUrl = constAmazonDomainUrl + HttpUtility.HtmlDecode(hrefValue); //"http://www.amazon.com/s/ref=sr_pg_2?rh=n%3A2625373011%2Cn%3A%212644981011%2Cn%3A%212644982011%2Cn%3A2858778011&page=2&ie=UTF8&qid=1368697688"

            gotNextPageUrl = true;
        }
        else
        {
            gotNextPageUrl = false;
        }

        return gotNextPageUrl;
    }

    /*
     * [Function]
     * extract searched item list from amazon search result html 
     * [Input]
     * amazon search result html
     * [Output]
     * searched item list, each type is searchResultItem
     * [Note]
     */
    public bool extractSearchItemList(string searchRespHtml, out List<searchResultItem> itemList)
    {
        bool extractItemListOk = false;

        itemList = new List<searchResultItem>();

        //type1:
        //<div id="atfResults" class="list results twister">
        //<div id="result_0" class="result firstRow product celwidget" name="B00CE18P0K">
        //<div id="result_1" class="result product celwidget" name="B00CL68QVQ">
        //<div id="result_2" class="result lastRow product celwidget" name="B008Y7N7JW">

        //<div id="btfResults" class="list results twister">
        //<div id="result_3" class="result product celwidget" name="B008XKSW7M">
        //...

        //type2:
        //<div id="result_24" class="fstRowGrid prod celwidget" name="B00CDURMD8">
        //<div id="result_25" class="fstRowGrid prod celwidget" name="B003AZBASI">
        //...

        HtmlAgilityPack.HtmlDocument htmlDoc = crl.htmlToHtmlDoc(searchRespHtml);
        HtmlNodeCollection resultItemNodeList;
        //resultItemNodeList = htmlDoc.DocumentNode.SelectNodes("//div[@id and @class and @name]");
        //resultItemNodeList = htmlDoc.DocumentNode.SelectNodes("//div[starts-with(@id, 'result_') and starts-with(@class, 'result ') and @name]");
        resultItemNodeList = htmlDoc.DocumentNode.SelectNodes("//div[starts-with(@id, 'result_') and @class and @name]");
        if (resultItemNodeList != null)
        {
            foreach (HtmlNode resultItemNode in resultItemNodeList)
            {
                crifanLibAmazon.searchResultItem curItem = new crifanLibAmazon.searchResultItem();
                //<h3 class="title"  ><a class="title" href="http://www.amazon.com/Pilot-HD/dp/B00CE18P0K/ref=sr_1_1?s=instant-video&amp;ie=UTF8&amp;qid=1368685217&amp;sr=1-1">Zombieland Season 1 [HD]</a> <span class="starring">Starring Kirk Ward,&#32;Tyler Ross,&#32;Maiara Walsh and Izabela Vidovic</span></h3>
                //<h3 class="newaps">    <a href="http://www.amazon.com/Pilot-HD/dp/B00CE18P0K/ref=sr_1_1?s=instant-video&amp;ie=UTF8&amp;qid=1369302177&amp;sr=1-1"><span class="lrg bold">Zombieland Season 1 [HD]</span></a> <span class="med reg"><span class="starring">Starring Kirk Ward,&#32;Tyler Ross,&#32;Maiara Walsh and Izabela Vidovic</span></span>    </h3>
                //HtmlNode h3aNode = resultItemNode.SelectSingleNode(".//h3[@class='title']/a");
                HtmlNode h3aNode = resultItemNode.SelectSingleNode(".//h3[@class]/a");
                if (h3aNode != null)
                {
                    string productUrl = h3aNode.Attributes["href"].Value;//"http://www.amazon.com/Pilot-HD/dp/B00CE18P0K/ref=sr_1_1?s=instant-video&amp;ie=UTF8&amp;qid=1368688342&amp;sr=1-1"
                    string decodedProductUrl = HttpUtility.HtmlDecode(productUrl);//"http://www.amazon.com/Silver-Linings-Playbook/dp/B00CL68QVQ/ref=sr_1_2?s=instant-video&ie=UTF8&qid=1368688342&sr=1-2"

                    curItem.productUrl = decodedProductUrl;

                    itemList.Add(curItem);

                    extractItemListOk = true;
                }
                else
                {
                    //something wrong
                }
            }
        }
        else
        {
            //something wrong
        }

        return extractItemListOk;
    }

    /*
     * [Function]
     * from html extract buyer number and used and new url
     * [Input]
     * amazon product url's html
     * [Output]
     * buyer number and used and new url
     * [Note]
     */
    public bool extractProductBuyerNumberAndNewUrl(string productHtml, out int totalBuyerNumber, out string usedAndNewUrl)
    {
        bool extractOk = false;

        totalBuyerNumber = 0;
        usedAndNewUrl = "";

        HtmlAgilityPack.HtmlDocument htmlDoc = crl.htmlToHtmlDoc(productHtml);
        HtmlNode rootNode = htmlDoc.DocumentNode;

        /*
          <div class="mbcContainer">
            <div class="mbcTitle">More Buying Choices</div>
            <div id="more-buying-choice-content-div">
        <div id="secondaryUsedAndNew" class="mbcOlp" style="text-align:center;">
        <div class="mbcOlpLink" ><a class="buyAction" href="/gp/offer-listing/B0083PWAPW/ref=dp_olp_all_mbc?ie=UTF8&condition=all">18&nbsp;used&nbsp;&&nbsp;new</a>&nbsp;from&nbsp;<span class="price">$180.00</span></div>
        </div>
            </div>
          </div>
         */
        HtmlNode mbcNode = rootNode.SelectSingleNode("//div[@class='mbcContainer']");
        if (mbcNode != null)
        {
            HtmlNode buyActionNode = mbcNode.SelectSingleNode("./div[@id='more-buying-choice-content-div']/div@[id='secondaryUsedAndNew']/div@[class='mbcOlpLink']/a[@class='buyAction']");
            if (buyActionNode != null)
            {
                //find url for "18 used & new "
                usedAndNewUrl = buyActionNode.Attributes["href"].Value; ///gp/offer-listing/B0083PWAPW/ref=dp_olp_all_mbc?ie=UTF8&condition=all
                usedAndNewUrl = constAmazonDomainUrl + usedAndNewUrl;//http://www.amazon.com/gp/offer-listing/B0083PWAPW/ref=dp_olp_all_mbc?ie=UTF8&condition=all

                string buyActionStr = buyActionNode.InnerText; //18&nbsp;used&nbsp;&&nbsp;new
                string buyNumberStr = "";
                if (crl.extractSingleStr(@"^(\d+)", buyActionStr, out buyNumberStr))
                {
                    int buyNumberInt = Int32.Parse(buyNumberStr); //18

                    extractOk = true;
                }
            }
        }

        return extractOk;
    }

    /*
     * [Function]
     * from amazon main url extract main category
     * [Input]
     * http://www.amazon.com/ref=nb_sb_noss_null
     * [Output]
     * categoryItem list, contains 36 main category:
     * Key	"instant-video"	string
     * Name	"Amazon Instant Video"	string
     * ...
     * Key	"watches"	string
     * Name	"Watches"	string
     * [Note]
     */
    public List<categoryItem> extractMainCategoryList(string amazonMainUrl)
    {
        List < categoryItem> mainCategoryList = new List<categoryItem>();

        string respHtml = "";
        //respHtml = crl.getUrlRespHtml(regularCategoryMainUrl);
        respHtml = crl.getUrlRespHtml_multiTry(amazonMainUrl);
        
        /*
        <span id='nav-search-in' class='nav-sprite'>
          <span id='nav-search-in-content' data-value="search-alias=aps">
            All
          </span>
          <span class='nav-down-arrow nav-sprite'></span>
          <select name="url" id="searchDropdownBox" class="searchSelect" title="Search in"   ><option value="search-alias=aps" selected="selected">All Departments</option><option value="search-alias=instant-video">Amazon Instant Video</option><option value="search-alias=appliances">Appliances</option><option value="search-alias=mobile-apps">Apps for Android</option><option value="search-alias=arts-crafts">Arts, Crafts & Sewing</option><option value="search-alias=automotive">Automotive</option><option value="search-alias=baby-products">Baby</option><option value="search-alias=beauty">Beauty</option><option value="search-alias=stripbooks">Books</option><option value="search-alias=mobile">Cell Phones & Accessories</option><option value="search-alias=apparel">Clothing & Accessories</option><option value="search-alias=collectibles">Collectibles</option><option value="search-alias=computers">Computers</option><option value="search-alias=financial">Credit Cards</option><option value="search-alias=electronics">Electronics</option><option value="search-alias=gift-cards">Gift Cards Store</option><option value="search-alias=grocery">Grocery & Gourmet Food</option><option value="search-alias=hpc">Health & Personal Care</option><option value="search-alias=garden">Home & Kitchen</option><option value="search-alias=industrial">Industrial & Scientific</option><option value="search-alias=jewelry">Jewelry</option><option value="search-alias=digital-text">Kindle Store</option><option value="search-alias=magazines">Magazine Subscriptions</option><option value="search-alias=movies-tv">Movies & TV</option><option value="search-alias=digital-music">MP3 Music</option><option value="search-alias=popular">Music</option><option value="search-alias=mi">Musical Instruments</option><option value="search-alias=office-products">Office Products</option><option value="search-alias=lawngarden">Patio, Lawn & Garden</option><option value="search-alias=pets">Pet Supplies</option><option value="search-alias=shoes">Shoes</option><option value="search-alias=software">Software</option><option value="search-alias=sporting">Sports & Outdoors</option><option value="search-alias=tools">Tools & Home Improvement</option><option value="search-alias=toys-and-games">Toys & Games</option><option value="search-alias=videogames">Video Games</option><option value="search-alias=watches">Watches</option></select>
        </span>
         */
        HtmlAgilityPack.HtmlDocument htmlDoc = crl.htmlToHtmlDoc(respHtml);
        HtmlNode categorySelectNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='nav-search-in' and @class='nav-sprite']/select[@name='url' and @id='searchDropdownBox' and @class='searchSelect']");
        if (categorySelectNode != null)
        {
            HtmlNodeCollection optionNodeList = categorySelectNode.SelectNodes(".//option[@value]");

            //omit first one:
            //<option value="search-alias=aps" selected="selected">All Departments</option>
            optionNodeList.Remove(0);

            foreach (HtmlNode singleOptionNode in optionNodeList)
            {
                //<option value="search-alias=instant-video">Amazon Instant Video</option>
                //<option value="search-alias=appliances">Appliances</option>
                //...
                //<option value="search-alias=watches">Watches</option>
                string searchValue = singleOptionNode.Attributes["value"].Value; //search-alias=instant-video
                string keyValue = ""; //instant-video
                if (crl.extractSingleStr(@"=([a-z\-]+)", searchValue, out keyValue))
                {
                    //instant-video
                    //appliances
                    //mobile-apps

                    string generalCategory = singleOptionNode.InnerText; //Amazon Instant Video
                    //string generalCategory = singleOptionNode.NextSibling.InnerText; //Amazon Instant Video

                    //store info
                    categoryItem singleCategoryItem = new categoryItem();
                    singleCategoryItem.Name = generalCategory;
                    singleCategoryItem.Key = keyValue;
                    //singleCategoryItem.name = generalCategory;
                    //singleCategoryItem.url = singleCategoryUrl;
                    //add to list
                    mainCategoryList.Add(singleCategoryItem);
                }
                else
                {

                }
            }
        }
        else
        {
 
        }

        return mainCategoryList;
    }

    /*
     * [Function]
     * from html extract product title
     * [Input]
     * http://www.amazon.com/Kindle-Fire-HD/dp/B0083PWAPW/ref=lp_1055398_1_1?ie=UTF8&qid=1369487181&sr=1-1
     * [Output]
     * Kindle Fire HD Tablet
     * [Note]
     */
    public string extractProductTitle(string respHtml)
    {
        string productTitle = "";
        //http://www.amazon.com/Kindle-Fire-HD/dp/B0083PWAPW/ref=lp_1055398_1_1?ie=UTF8&qid=1369487181&sr=1-1
        //<span id="btAsinTitle">Kindle Fire HD Tablet</span>

        //http://www.amazon.com/Kindle-Paperwhite-Touch-light/dp/B007OZNZG0/ref=lp_1055398_1_2?ie=UTF8&qid=1369487181&sr=1-2
        //<span id="btAsinTitle">Kindle Paperwhite</span>

        //http://www.amazon.com/Garmin-5-Inch-Portable-Navigator-Lifetime/dp/B0057OCDQS/ref=lp_1055398_1_3?ie=UTF8&qid=1369487181&sr=1-3
        //<span id="btAsinTitle">Garmin nüvi 50LM 5-Inch Portable GPS Navigator with Lifetime Maps (US)</span>

        //http://www.amazon.com/GE-MWF-Refrigerator-Filter-1-Pack/dp/B000AST3AK/ref=lp_1055398_1_4?ie=UTF8&qid=1369487181&sr=1-4
        //<span id="btAsinTitle">GE MWF Refrigerator Water Filter, 1-Pack</span>
        HtmlAgilityPack.HtmlDocument htmlDoc = crl.htmlToHtmlDoc(respHtml);
        HtmlNode titleNode = htmlDoc.DocumentNode.SelectSingleNode("//span[@id='btAsinTitle']");
        if (titleNode != null)
        {
            productTitle = titleNode.InnerText; //Kindle Paperwhite
        }
        else
        {
            //something wrong 
        }
        return productTitle;
    }

    /*
     * [Function]
     * from html extract product bullets
     * [Input]
     * html of http://www.amazon.com/GE-MWF-Refrigerator-Filter-1-Pack/dp/B000AST3AK/ref=lp_1055398_1_3?ie=UTF8&qid=1369726182&sr=1-3
     * [Output]
     * string list:
     *  Replacement water filter for refrigerators
     *  Works with a wide-range of GE models
     *  Replaces GWF, GWFA, GWF01, GWF06, MWFA
     *  Eliminates home delivery/store bought bottled water
     *  Should be replaced every 6 months
     * [Note]
     * 1. normally, only have 5 bullets
     */
    public bool extractProductBulletList(string respHtml, out List<string> bulletList)
    {
        bool gotBullets = false;

        bulletList = new List<string>();
        
        HtmlAgilityPack.HtmlDocument htmlDoc = crl.htmlToHtmlDoc(respHtml);
        HtmlNode rootNode = htmlDoc.DocumentNode;

        //-----------------bullets-----------------
        //http://www.amazon.com/Kindle-Fire-HD/dp/B0083PWAPW/ref=lp_1055398_1_1?ie=UTF8&qid=1369487181&sr=1-1
        //<div id="kindle-feature-bullets-atf">
        //  <div>
        //    <ul>
        //        <li><span>1280x800 HD display with polarizing filter and anti-glare technology for rich color and deep contrast from any viewing angle</span></li><li><span>Exclusive Dolby audio and dual-driver stereo speakers for immersive, virtual surround sound</span></li><li><span>World's first tablet with dual-band, dual-antenna Wi-Fi for over 35% faster downloads and streaming (<a href="#" id="kpp-popover-0" >compared to the iPad mini</a><script type="text/javascript">
        //amznJQ.available('jQuery', function() { 
        //(function ($) {
        //amznJQ.available('popover', function() {
        //    var content = '<h2 style="font-size: 17px;">Two Antennas, Better Bandwidth</h2>' 

        //    + '<img src="http://g-ec2.images-amazon.com/images/G/01/kindle/dp/2012/KT/tate_feature-wifi._V395653267_.gif"/>'

        //    $('#kpp-popover-0').amazonPopoverTrigger({
        //        literalContent: content,
        //        closeText: 'Close',
        //        title: '&nbsp;',
        //        width: 550,
        //        location: 'centered'
        //    });

        //});
        //}(jQuery)); 
        //}); 

        //</script>)</span></li><li><span>High performance 1.2 Ghz dual-core processor with Imagination PowerVR 3D graphics core for fast and fluid performance</span></li><li><span>Over 23 million movies, TV shows, songs, magazines, books, audiobooks, and popular apps and games such as <i>Facebook</i>, <i>Netflix</i>, <i>Twitter</i>, <i>HBO GO</i>, <i>Pandora</i>, and <i>Angry Birds Space</i></span></li><li><span>Integrated support for Facebook, Twitter, Gmail, Hotmail, Yahoo! and more, as well as Exchange calendar, contacts, and email</span></li><li><span>Front-facing HD camera for taking photos or making video calls using Skype, Facebook, and other apps</span></li><li><span>Free unlimited cloud storage for all your Amazon content</span></li><li><span>Kindle FreeTime &mdash; a free, personalized tablet experience just for kids on the Kindle Fire HD. Set daily screen limits, and give access to appropriate content for each child</span></li><li><span>Kindle FreeTime Unlimited &mdash; just for kids. Unlimited access to books, games, apps, movies and TV shows. <a href="http://www.amazon.com/gp/feature.html?&docId=1000863021" target="_blank">Learn more</a></span></li><li><span><img src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/prime.gif"/> Prime Instant Video &mdash; unlimited, instant streaming of thousands of popular movies and TV shows</span></li><li><span><img src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/prime.gif"/> Kindle Owners' Lending Library &mdash; Kindle owners can choose from more than 270,000 books to borrow for free with no due dates, including over 100 current and former <i>New York Times</i> best sellers</span></li><li><span><b>NEW:</b> Kindle Fire owners get 500 <b>Amazon Coins</b> (a $5 value) to spend on Kindle Fire apps and games. <a href="http://www.amazon.com/gp/feature.html/ref=zeroes_surl_c_landing?docId=1001166401" target="_blank">Learn more</a></span></li>
        //    </ul>
        //  </div>
        //</div>

        //http://www.amazon.com/Kindle-Paperwhite-Touch-light/dp/B007OZNZG0/ref=lp_1055398_1_2?ie=UTF8&qid=1369487181&sr=1-2
        //<div id="kindle-feature-bullets-atf">
        //  <div>
        //    <ul>
        //        <li><span>Patented built-in light evenly illuminates the screen to provide the perfect reading experience in all lighting conditions</span></li><li><span>Paperwhite has 62% more pixels for brilliant resolution</span></li><li><span>25% better contrast for sharp, dark text</span></li><li><span>Even in bright sunlight, Paperwhite delivers clear, crisp text and images with no glare</span></li><li><span>New hand-tuned fonts - 6 font styles, 8 adjustable sizes</span></li><li><span>8-week battery life, even with the light on</span></li><li><span>Holds up to 1,100 books - take your library wherever you go</span></li><li><span>Built-in Wi-Fi lets you download books in under 60 seconds</span></li><li><span>New Time to Read feature uses your reading speed to let you know when you'll finish your chapter</span></li><li><span>Massive book selection. Lowest prices. Over a million titles less than $9.99</span></li><li><span>180,000 Kindle-exclusive titles that you won't find anywhere else, including books by best-selling authors such as Kurt Vonnegut</span></li><li><span>Supports children's books and includes new parental controls</span></li><li><span><img src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KS/prime.gif" /> Kindle Owners' Lending Library - with Amazon Prime, Kindle owners can choose from more than 270,000 books to borrow for free with no due dates, including over 100 current and former <i>New York Times</i> best sellers</span></li>
        //    </ul>
        //  </div>
        //</div>


        //http://www.amazon.com/GE-MWF-Refrigerator-Filter-1-Pack/dp/B000AST3AK/ref=lp_1055398_1_3?ie=UTF8&qid=1369726182&sr=1-3
        //<div id="feature-bullets-atf">
        //<table cellpadding="0" cellspacing="0" border="0">
        //  <tr>
        //    <td class="bucket normal" style="padding:0px">
        //      <div class="content" style="margin-left:15px;">
        //        <ul style="list-style-type: disc;">
        //      <li><span>Replacement water filter for refrigerators</span></li>
        //      <li><span>Works with a wide-range of GE models</span></li>
        //      <li><span>Replaces GWF, GWFA, GWF01, GWF06, MWFA</span></li>
        //      <li><span>Eliminates home delivery/store bought bottled water</span></li>
        //      <li><span>Should be replaced every 6 months</span></li>
        //        </ul>
        //      </div>
        //    <span class="caretnext">&#155;</span>
        //       <a href="#productDetails" style="font-size:12px">
        //    See more product details </a>
        //    </td>
        //  </tr>
        //</table>
        //</div>

        //http://www.amazon.com/Garmin-5-Inch-Portable-Navigator-Lifetime/dp/B0057OCDQS/ref=lp_1055398_1_6?ie=UTF8&qid=1369727413&sr=1-6
        //<div id="feature-bullets_feature_div">
        //<hr noshade="noshade" size="1" class="bucketDivider" />
        //<table cellpadding="0" cellspacing="0" border="0">
        //  <tr>
        //    <td class="bucket normal">
        //      <h2>Product Features</h2>
        //    <div class="disclaim">Edition: <strong>5-Inch with Lifetime Maps</strong></div>
        //          <div class="content">
        //        <ul style="list-style-type: disc; margin-left: 25px;">
        //       <li>5-inch LCD display,Memory Card Supported: microSD Card</li>
        //       <li>Free lifetime maps with over 6 million points of interest; Hear spoken street names</li>
        //       <li>Speed limit indicator</li>
        //       <li>USB mass storage device is compatible with Windows XP or newer and Mac OS X 10.4 or later</li>
        //       <li>Trip computer records mileage, max speed, total time and more,Lane assist with junction view</li>
        //        </ul>
        //        <div>
        //            <span class="caretnext">&#155;</span>
        //                    <a href="#productDetails">
        //    See more product details </a>
        //                </div>
        //      </div>
        //    </td>
        //  </tr>
        //</table>
        //</div>

        //http://www.amazon.com/SODIAL-Colorful-Magnetic-Numbers-Educational/dp/B008RRPLN4/ref=sr_1_58?s=home-garden&ie=UTF8&qid=1369730932&sr=1-58
        //<table cellpadding="0" cellspacing="0" border="0">
        //  <tr>
        //    <td class="bucket normal">
        //      <h2>Product Features</h2>
        //      <div class="content">
        //        <ul style="list-style-type: disc; margin-left: 25px;">
        //       <li>Numbers Wooden Fridge Magnets</li>
        //       <li>Funky Fridge Magnets</li>
        //       <li>Fridge Colorful Magnetic</li>
        //       <li>Numbers Magnetic</li>
        //       <li>Fridge Magnets</li>
        //        </ul>
        //      </div>
        //    </td>
        //  </tr>
        //</table>

        //http://www.amazon.com/Tech-Armor-Definition-Protectors-Replacement/dp/B00BT7RAPG/ref=sr_1_4?s=wireless&ie=UTF8&qid=1369754056&sr=1-4
        //<div class="bucket">
        //  <a name="technical_details" id="technical_details"></a><h2>Technical Details</h2>   
        //  <div class="content">
        //<ul style="list-style: disc; margin-left: 25px;">
        //<li>High Definition Transparency Film that ensures maximum resolution for your Galaxy S4</li>
        //<li>TruTouch Sensitivity for a natural feel that provides flawless touch screen accuracy</li>
        //<li>Protects your Samsung AMOLED Display from unwanted scratches</li>
        //<li>Repels dust and will reduce signs of daily wear</li>
        //<li>Made from the highest quality Japanese PET Film with 100% Bubble-Free Adhesives for easy installation and no residue when removed</li></ul>
        //<span class="caretnext">&#155;</span>&nbsp;
        //<a href="http://www.amazon.com/Tech-Armor-Definition-Protectors-Replacement/dp/tech-data/B00BT7RAPG/ref=de_a_smtd">See more technical details</a>
        // </div>
        //</div>


        //------ for bullets ------
        HtmlNode bulletsNode = null;
        HtmlNodeCollection bulletsNodeList = null;
        string bulletXpath = "";

        if (bulletsNode == null)
        {
            //<div id="feature-bullets_feature_div">
            //<div id="kindle-feature-bullets-atf">
            HtmlNode featureBulletsAtfNode = rootNode.SelectSingleNode("//div[contains(@id,'feature-bullets-atf')]");
            if (featureBulletsAtfNode != null)
            {
                bulletsNode = featureBulletsAtfNode;
                //<li><span>1280x800 HD display with polarizing filter and anti-glare technology for rich color and deep contrast from any viewing angle</span></li>
                bulletXpath = ".//li/span";
            }
        }

        if (bulletsNode == null)
        {
            //<div id="feature-bullets_feature_div">
            HtmlNode featureBulletsDivNode = rootNode.SelectSingleNode("//div[@id='feature-bullets_feature_div']");
            if (featureBulletsDivNode != null)
            {
                bulletsNode = featureBulletsDivNode;
                //<li>5-inch LCD display,Memory Card Supported: microSD Card</li>
                bulletXpath = ".//li";
            }
        }

        if (bulletsNode == null)
        {
            //<td class="bucket normal">
            HtmlNode tdBucketNormalNode = rootNode.SelectSingleNode("//td[@class='bucket normal']");
            if (tdBucketNormalNode != null)
            {
                bulletsNode = tdBucketNormalNode;
                //<li>Numbers Wooden Fridge Magnets</li>
                bulletXpath = ".//li";
            }
        }

        if (bulletsNode == null)
        {
            //<div class="bucket">
            HtmlNode bucketDivNode = rootNode.SelectSingleNode("//div[@class='bucket']");
            if (bucketDivNode != null)
            {
                bulletsNode = bucketDivNode;
                //<li>High Definition Transparency Film that ensures maximum resolution for your Galaxy S4</li>
                bulletXpath = ".//li";
            }
        }
        
        //finnal process
        if (bulletsNode != null)
        {
            bulletsNodeList = bulletsNode.SelectNodes(bulletXpath);

            //special:
            //maybe has more than 5 bullets
            //http://www.amazon.com/AmazonBasics-Lightning-Compatible-Cable-inch/dp/B00B5RGAWY/ref=sr_1_3?s=wireless&ie=UTF8&qid=1369753764&sr=1-3
            //has feature-bullets_feature_div, but no content -> bulletsNodeList is null
            if (bulletsNodeList != null)
            {
                gotBullets = true;

                for (int idx = 0; idx < bulletsNodeList.Count; idx++)
                {
                    HtmlNode curBulletNode = bulletsNodeList[idx];

                    HtmlNode noJsNode = crl.removeSubHtmlNode(curBulletNode, "script");
                    HtmlNode noStyleNode = crl.removeSubHtmlNode(curBulletNode, "style");

                    string bulletStr = noStyleNode.InnerText;
                    bulletList.Add(bulletStr);
                }
            }
            else
            {
                //something wrong
            }
        }
        else 
        {
            //some indeed no bullets
            //but maybe some has, but fail to find -> wrong
        }

        return gotBullets;
    }

    /*
     * [Function]
     * from html extract product description
     * [Input]
     * html of http://www.amazon.com/Legend-Zelda-Hyrule-Historia/dp/1616550414/ref=lp_1_1_1?ie=UTF8&qid=1367990173&sr=1-1
     * [Output]
     * Dark Horse Books and Nintendo team up to bring you The Legend of Zelda: Hyrule Historia, containing an unparalleled collection of historical information on The Legend of Zelda franchise. This handsome hardcover contains never-before-seen concept art, the full history of Hyrule, the official chronology of the games, and much more! Starting with an insightful introduction by the legendary producer and video-game designer of Donkey Kong, Mario, and The Legend of Zelda, Shigeru Miyamoto, this book is crammed full of information about the storied history of Link's adventures from the creators themselves! As a bonus, The Legend of Zelda: Hyrule Historia includes an exclusive comic by the foremost creator of The Legend of Zelda manga - Akira Himekawa!
     * [Note]
     * 1. normally, only have description, some special not have
     */
    public bool extractProductDescription(string respHtml, out string description)
    {
        bool isFoundDescription = false;
        description = "";

        HtmlAgilityPack.HtmlDocument htmlDoc = crl.htmlToHtmlDoc(respHtml);
        HtmlNode rootNode = htmlDoc.DocumentNode;
        
        //-----------------description-----------------
        //http://www.amazon.com/Garmin-5-Inch-Portable-Navigator-Lifetime/dp/B0057OCDQS/ref=lp_1055398_1_3?ie=UTF8&qid=1369487181&sr=1-3
        //<div class="bucket" id="productDescription">
        // <h2>Product Description</h2>
        //    <div class="disclaim">Edition: <strong>5-Inch with Lifetime Maps</strong></div>
        //     <div class="content">
        //           <h3 class="productDescriptionSource" ></h3>
        //       <div class="productDescriptionWrapper" >
        //       With a big 5&rdquo; (12.7 cm) touchscreen, more than 5 million points of  <img src="http://g-ecx.images-amazon.com/images/G/01/B005HSDL8S/cf-lg-5.jpg" style="float: right;" />interest (POIs) and spoken turn-by-turn directions, n&uuml;vi 50LM makes  driving fun again. Plus, with FREE lifetime map updates, you always can  keep your roads and POIs up to date.<br /><br /> <h3>Get Turn-by-Turn Directions</h3> <p>n&uuml;vi 50LM's intuitive interface greets you with 2 simple choices:  "Where To?" and "View Map." Touch the screen to easily look up addresses  and services and to be guided to your destination with voice-prompted,  turn-by-turn directions that speak street names. It comes in 2 mapping  versions and has preloaded maps for the lower 48 states plus Hawaii and  Puerto Rico. n&uuml;vi 50LM&rsquo;s speed limit indicator shows you how fast you can go  on most major roads. With its "Where Am I?" emergency locator, you  always know your location. It also comes preloaded with millions of POIs  and offers the ability to add your own.</p> <h3>Enjoy FREE Lifetime Map Updates</h3> <p><img src="http://g-ecx.images-amazon.com/images/G/01/B005HSDL8S/lf-lg-4.jpg" style="float: left;" />With FREE lifetime map&sup1; updates, you always have the most up-to-date  maps, POIs and navigation information available at your fingertips. Map  updates are available for download up to 4 times a year with no  subscription or update fees and no expiration dates.</p> <h3>Know the Lane Before It&rsquo;s Too Late</h3> <p>Now there&rsquo;s no more guessing which lane you need to be in to make an  upcoming turn. Available in select metropolitan areas, lane assist with  junction view guides you to the correct lane for an approaching turn or  exit, making unfamiliar intersections and exits easy to navigate. It  realistically displays road signs and junctions on your route along with  arrows that indicate the proper lane for navigation. <img src="http://g-ecx.images-amazon.com/images/G/01/B005HSDL8S/pd-01-lg-4.jpg" style="float: right;" /></p> <p><em>&sup1; FREE lifetime map updates entitle you to receive up to 4 map  data updates per year, when and as such updates are made available on  the Garmin website, for this specific Garmin product only until this  product&rsquo;s useful life expires or Garmin no longer receives map data from  its third party supplier, whichever is shorter. The updates you receive  will be updates to the same geographic map data originally included  with your Garmin product when originally purchased. Garmin may terminate  your lifetime map updates at any time if you violate any of the terms  of the End User License Agreement accompanying your n&uuml;vi product. <br /></em></p> <p><em></em></p> <h3>What's in the Box:</h3> <ul> <li>n&uuml;vi 50LM</li> <li>City Navigator&reg; NT  data with preloaded street maps of the lower 48 states, Hawaii, Puerto  Rico, U.S. Virgin Islands, Cayman Islands, Bahamas, French Guiana,  Guadeloupe, Martinique, Saint Barth&eacute;lemy and Jamaica </li> <li>Lifetime maps&sup1; (indicated by "LM" after model number on the box)</li> <li>Vehicle suction cup mount&sup2;</li> <li>Vehicle power cable</li> <li>USB cable</li> <li>Quick start manual</li> </ul>
        //      <div class="emptyClear"> </div>
        //      </div>
        //  </div>
        //</div>

        //http://www.amazon.com/Legend-Zelda-Hyrule-Historia/dp/1616550414/ref=lp_1_1_1?ie=UTF8&qid=1367990173&sr=1-1
        //<div id="ps-content" class="bucket">
        //  <h2>Book Description</h2>
        //<div class="buying"><span class="byLinePipe">Release date: </span><span style="font-weight: bold;">January 29, 2013</span> </div>
        //  <div class="content">
        //    <div id="outer_postBodyPS" style="overflow:hidden; z-index: 1; ">
        //      <div id="postBodyPS" style="overflow: hidden;">
        //         <div>Dark Horse Books and Nintendo team up to bring you The Legend of Zelda: Hyrule Historia, containing an unparalleled collection of historical information on The Legend of Zelda franchise. This handsome hardcover contains never-before-seen concept art, the full history of Hyrule, the official chronology of the games, and much more! Starting with an insightful introduction by the legendary producer and video-game designer of Donkey Kong, Mario, and The Legend of Zelda, Shigeru Miyamoto, this book is crammed full of information about the storied history of Link's adventures from the creators themselves! As a bonus, The Legend of Zelda: Hyrule Historia includes an exclusive comic by the foremost creator of The Legend of Zelda manga - Akira Himekawa!</div>
        //      </div>
        //    </div>
        //    <div id="psGradient" class="psGradient" style="display:none;"></div>
        //    <div id="psPlaceHolder" style="display:none; height: 20px;">
        //      <div id="expandPS" style="display:none; z-index: 3;">
        //        <span class="swSprite s_expandChevron"></span>
        //        <a class="showMore" onclick="amz_expandPostBodyDescription('PS', ['psGradient', 'psPlaceHolder']); return false;" href="#">Show more</a>
        //      </div>
        //    </div>
        //    <div id="collapsePS" style="display:none; padding-top: 3px;">
        //      <span class="swSprite s_collapseChevron"></span>
        //      <a class="showLess" onclick="amz_collapsePostBodyDescription('PS', ['psGradient', 'psPlaceHolder']); return false;" href="#">Show less</a>
        //    </div>
        //<noscript>
        //  <style type='text/css'>
        //    #outer_postBodyPS {
        //      display: none;
        //    }
        //    #psGradient {
        //      display: none;
        //    }
        //    #psPlaceHolder {
        //      display: none;
        //    }
        //    #psExpand {
        //      display: none;
        //    }
        //  </style>
        //    <div id="postBodyPS">Dark Horse Books and Nintendo team up to bring you The Legend of Zelda: Hyrule Historia, containing an unparalleled collection of historical information on The Legend of Zelda franchise. This handsome hardcover contains never-before-seen concept art, the full history of Hyrule, the official chronology of the games, and much more! Starting with an insightful introduction by the legendary producer and video-game designer of Donkey Kong, Mario, and The Legend of Zelda, Shigeru Miyamoto, this book is crammed full of information about the storied history of Link's adventures from the creators themselves! As a bonus, The Legend of Zelda: Hyrule Historia includes an exclusive comic by the foremost creator of The Legend of Zelda manga - Akira Himekawa!</div>
        //</noscript>
        //  </div>
        //</div>


        //------ for description ------
        HtmlNode descriptionNode = null;
        HtmlNode filteredDescriptionNode = null;
        if (descriptionNode == null)
        {
            //<div id="ps-content" class="bucket">
            // <div class="content">
            //  <div id="outer_postBodyPS" style="overflow:hidden; z-index: 1; ">
            //   <div id="postBodyPS" style="overflow: hidden;">
            //    <div>
            HtmlNode postBodyNode = rootNode.SelectSingleNode(
                "//div[@class='bucket']/div[@class='content']/div[@id='outer_postBodyPS']/div[@id='postBodyPS']/div");
            if (postBodyNode != null)
            {
                descriptionNode = postBodyNode;
            }
        }

        if (descriptionNode == null)
        {
            //<div class="bucket" id="productDescription">
            // <div class="content">
            //  <div class="productDescriptionWrapper" >
            HtmlNode postBodyNode = rootNode.SelectSingleNode(
                "//div[@class='bucket']/div[@class='content']/div[@class='productDescriptionWrapper']");
            if (postBodyNode != null)
            {
                descriptionNode = postBodyNode;
            }
        }
        
        //finnal process
        if (descriptionNode == null)
        {
            isFoundDescription = false;
        }
        else
        {
            HtmlNode noPNode = crl.removeSubHtmlNode(descriptionNode, "p");
            HtmlNode noScriptNode = crl.removeSubHtmlNode(noPNode, "script");
            HtmlNode noStyleNode = crl.removeSubHtmlNode(noScriptNode, "style");
            HtmlNode noTableNode = crl.removeSubHtmlNode(noStyleNode, "table");

            filteredDescriptionNode = noTableNode;

            //description
            description = filteredDescriptionNode.InnerText;

            isFoundDescription = true;
        }

        return isFoundDescription;
    }

    /*
     * [Function]
     * from html extract image url list
     * [Input]
     * html of http://www.amazon.com/Kindle-Fire-HD/dp/B0083PWAPW/ref=lp_1055398_1_2?ie=UTF8&qid=1369820725&sr=1-2
     * [Output]
     * image url list:
     * http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-01-lg._V395919237_.jpg 
     * http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-apps-lg._V396577301_.jpg
     * http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-web-lg._V396577300_.jpg
     * http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-hd-lg._V396577300_.jpg
     * http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-social-lg._V396577301_.jpg
     * 
     * [Note]
     * 1. normally, only have 5 pic
     * 2.special has 7 pic:
     * http://www.amazon.com/Pandamimi-Dexule-Fashion-2-Piece-Protector/dp/B008TO8L1Y/ref=sr_1_79?s=wireless&ie=UTF8&qid=1369754594&sr=1-79
     * http://www.amazon.com/All-Ware-Stainless-Pineapple-De-Corer/dp/B000GA53CO/ref=lp_1055398_1_3?ie=UTF8&qid=1370071787&sr=1-3
     * 3. special:
     * (1)
     * http://www.amazon.com/Maytag-UKF8001-Refrigerator-Filter-1-Pack/dp/B001XW8KW4/ref=lp_1055398_1_4?ie=UTF8&qid=1370072907&sr=1-4
     * extract from "var colorImages" can got 1 pic
     * but extract from: <div id="main-image-fixed-container">, can got 3 pics
     * (2)
     * http://www.amazon.com/San-Francisco-Bay-Coffee-80-Count/dp/B007Y59HVM/ref=lp_1055398_1_5?ie=UTF8&qid=1370072907&sr=1-5
     * extract from "var colorImages" can got 6 pic
     * but total has 7 pics
     * (3)
     * http://www.amazon.com/Garmin-5-Inch-Portable-Navigator-Lifetime/dp/B0057OCDQS/ref=lp_1055398_1_6?ie=UTF8&qid=1370072907&sr=1-6
     * has 6 pics,
     * but extract from "var colorImages" can got 4 pic
     */
    public string[] extractProductImageList(string respHtml)
    {
        string[] imageUrlList = null;

        //------------------------- Method 1: find div kib-ma-container -------------------------
        //-> got node no contain img tag

        //http://www.amazon.com/Kindle-Paperwhite-Touch-light/dp/B007OZNZG0/ref=lp_1055398_1_1?ie=UTF8&qid=1369818181&sr=1-1
        //from webbrowser:
        //<div id="kib-ma-container-0" class="kib-ma-container" style="z-index: 1;"><div style="position: relative; float: left;"><div style="position: relative;" id="preplayDivmJRPXDWU3S51F"><div style="width: 500px; height: 483px;" class="outercenterslate"><div style="width:500px;height:0" class="centerslate"><span></span><img style="width: 500px; height: 483px;" src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-01-lg._V401028090_.jpg" border="0"></div><div src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-play-btn-off._V389377323_.gif" class="shuttleGradient" style="background: none;height:0px;"><img id="mJRPXDWU3S51FpreplayImageId" style="height:60px;position:absolute;left:0px;top:-60px;" src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-play-btn-off._V389377323_.gif" border="0"></div></div></div><div style="overflow: hidden; background: none repeat scroll 0% 0% rgb(0, 0, 0); width: 0px; height: 1px;" id="flashDivmJRPXDWU3S51F"><div id="so_mJRPXDWU3S51F"></div></div></div></div>
        //<div id="kib-ma-container-1" class="kib-ma-container" style="z-index: 0;"><div style="position: relative; float: left;"><div style="position: relative;" id="preplayDivm26TT75OS8GNBU"><div style="width: 500px; height: 483px;" class="outercenterslate"><div style="width:500px;height:0" class="centerslate"><span></span><img style="width: 500px; height: 483px;" src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-02-lg._V389678398_.jpg" border="0"></div><div src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-play-btn-off._V389377323_.gif" class="shuttleGradient" style="background: none;height:0px;"><img id="m26TT75OS8GNBUpreplayImageId" style="height:60px;position:absolute;left:0px;top:-60px;" src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-play-btn-off._V389377323_.gif" border="0"></div></div></div><div style="overflow: hidden; background: none repeat scroll 0% 0% rgb(0, 0, 0); width: 0px; height: 1px;" id="flashDivm26TT75OS8GNBU"><div id="so_m26TT75OS8GNBU"></div></div></div></div>
        //<div id="kib-ma-container-2" class="kib-ma-container" style="z-index: 0; display: none;">
        //    <img style="width: 500px; height: 483px;" class="kib-ma kib-image-ma" alt="Kindle Paperwhite e-reader" src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-03-lg._V400694812_.jpg" height="483" width="500">
        //</div>
        //<div id="kib-ma-container-3" class="kib-ma-container" style="z-index: 0; display: none;">
        //    <img style="width: 500px; height: 483px;" class="kib-ma kib-image-ma" alt="Kindle Paperwhite e-reader" src="http://g-ecx.images-amazon.com/images/G/01//kindle/dp/2012/KC/KC-slate-04-lg.jpg" height="483" width="500">
        //</div>
        //<div id="kib-ma-container-4" class="kib-ma-container" style="z-index: 0; display: none;">
        //    <img style="width: 500px; height: 483px;" class="kib-ma kib-image-ma" alt="Kindle Paperwhite 3G: thinner than a pencil" src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-05-lg._V389396235_.jpg" height="483" width="500">
        //</div>
        //from debug:
        //<div id="kib-ma-container-0" class="kib-ma-container" style="z-index:1;">
        //   <img src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-01-lg._V401028090_.jpg" width="500" height="483" style="margin-bottom:0px;" />
        //</div>
        //<div id="kib-ma-container-1" class="kib-ma-container" style="z-index:0;">
        //</div>
        //<div id="kib-ma-container-2" class="kib-ma-container" style="margin-bottom:1pxpx; z-index:0; display:none;">
        //    <img class="kib-ma kib-image-ma" alt="Kindle Paperwhite e-reader" src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-03-lg._V400694812_.jpg" width="500" height="483" />
        //</div>
        //<div id="kib-ma-container-3" class="kib-ma-container" style="margin-bottom:1pxpx; z-index:0; display:none;">
        //    <img class="kib-ma kib-image-ma" alt="Kindle Paperwhite e-reader" src="http://g-ecx.images-amazon.com/images/G/01//kindle/dp/2012/KC/KC-slate-04-lg.jpg" width="500" height="483" />
        //</div>
        //<div id="kib-ma-container-4" class="kib-ma-container" style="margin-bottom:1pxpx; z-index:0; display:none;">
        //    <img class="kib-ma kib-image-ma" alt="Kindle Paperwhite 3G: thinner than a pencil" src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-05-lg._V389396235_.jpg" width="500" height="483" />
        //</div>


        //http://www.amazon.com/Kindle-Fire-HD/dp/B0083PWAPW/ref=lp_1055398_1_2?ie=UTF8&qid=1369820725&sr=1-2
        //<div id="kib-ma-container-0" class="kib-ma-container" style="z-index:1;">  
        //   <img src="http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-01-lg._V395919237_.jpg" width="500" height="483" style="margin-bottom:0px;" />
        //</div> 
        //<div id="kib-ma-container-1" class="kib-ma-container" style="z-index:0;">
        //</div>
        //<div id="kib-ma-container-2" class="kib-ma-container" style="z-index:0;">
        //</div>
        //<div id="kib-ma-container-3" class="kib-ma-container" style="z-index:0;">
        //</div>
        //<div id="kib-ma-container-4" class="kib-ma-container" style="z-index:0;">
        //</div>


        //HtmlAgilityPack.HtmlDocument htmlDoc = crl.htmlToHtmlDoc(respHtml);
        //HtmlNode rootNode = htmlDoc.DocumentNode;

        //HtmlNodeCollection kibMaNodeList = rootNode.SelectNodes("//div[contains(@id, 'kib-ma-container-') and @class='kib-ma-container']");

        //if (kibMaNodeList != null)
        //{
        //    //for each, found first img -> real large pic
        //    //foreach (HtmlNode kibMaNode in kibMaNodeList)
        //    for (int idx = 0; idx < kibMaNodeList.Count; idx++)
        //    {
        //        HtmlNode kibMaNode = kibMaNodeList[idx];
        //        HtmlNode imgNode = kibMaNode.SelectSingleNode(".//img");
        //        if (imgNode != null)
        //        {
        //            string picUrl = imgNode.Attributes["src"].Value; //"http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-01-lg._V401028090_.jpg"
        //            imageUrlList[idx] = picUrl;
        //        }
        //        else
        //        {
        //            //something wrong

        //            //special one, no image:
        //            //<div id="kib-ma-container-1" class="kib-ma-container" style="z-index:0;">
        //            //</div>
        //        }
        //    }
        //}
        //else
        //{
        //    //something wrong
        //}


        //------------------------- Method 2: json to dict -------------------------

        //http://www.amazon.com/Kindle-Fire-HD/dp/B0083PWAPW/ref=lp_1055398_1_2?ie=UTF8&qid=1369820725&sr=1-2
        //each preplayImages->L, can got real large pic
        //<script type="text/javascript">
        //window.kibMAs = [
        //{
        //  "type" : "video", 
        //  "mediaObjectId" : "m1X6Z4SRW3DC3U",
        //  "richMediaObjectId" : "",
        //  "preplayImages" : {
        //      "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-01-lg._V395919237_.jpg", 
        //      "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-01-sm._V401027115_.jpg"
        //  },
        //  "html5PreferPosterHeight" : false,
        //  "thumbnailImageUrls" : {
        //      "default" : "http://g-ecx.images-amazon.com/images/G/01/kindle/whitney/dp/KW-imv-qt-tn._V167698598_.gif",
        //      "selected" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-tour-tn._V396577301_.jpg"
        //  }
        //}
        //,
        //{
        //  "type" : "video", 
        //  "mediaObjectId" : "m25IN8SS7SF6O1",
        //  "richMediaObjectId" : "",
        //  "preplayImages" : {
        //      "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-apps-lg._V396577301_.jpg", 
        //      "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-apps-sm._V396577300_.jpg"
        //  },
        //  "html5PreferPosterHeight" : false,
        //  "thumbnailImageUrls" : {
        //      "default" : "http://g-ecx.images-amazon.com/images/G/01/kindle/whitney/dp/KW-imv-qt-tn._V167698598_.gif",
        //      "selected" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-apps-tn._V396577301_.jpg"
        //  }
        //}
        //,
        //{
        //  "type" : "video", 
        //  "mediaObjectId" : "m1STLVYO0U0INQ",
        //  "richMediaObjectId" : "",
        //  "preplayImages" : {
        //      "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-web-lg._V396577300_.jpg", 
        //      "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-web-sm._V396577306_.jpg"
        //  },
        //  "html5PreferPosterHeight" : false,
        //  "thumbnailImageUrls" : {
        //      "default" : "http://g-ecx.images-amazon.com/images/G/01/kindle/whitney/dp/KW-imv-qt-tn._V167698598_.gif",
        //      "selected" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-web-tn._V396577306_.jpg"
        //  }
        //}
        //,
        //{
        //  "type" : "video", 
        //  "mediaObjectId" : "m3CHUVJSUUOBU4",
        //  "richMediaObjectId" : "",
        //  "preplayImages" : {
        //      "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-hd-lg._V396577300_.jpg", 
        //      "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-hd-sm.jpg"
        //  },
        //  "html5PreferPosterHeight" : false,
        //  "thumbnailImageUrls" : {
        //      "default" : "http://g-ecx.images-amazon.com/images/G/01/kindle/whitney/dp/KW-imv-qt-tn._V167698598_.gif",
        //      "selected" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-hd-tn._V396577306_.jpg"
        //  }
        //}
        //,
        //{
        //  "type" : "video", 
        //  "mediaObjectId" : "m3IVTAT62XST8A",
        //  "richMediaObjectId" : "",
        //  "preplayImages" : {
        //      "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-social-lg._V396577301_.jpg", 
        //      "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-social-sm.jpg"
        //  },
        //  "html5PreferPosterHeight" : false,
        //  "thumbnailImageUrls" : {
        //      "default" : "http://g-ecx.images-amazon.com/images/G/01/kindle/whitney/dp/KW-imv-qt-tn._V167698598_.gif",
        //      "selected" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-shme-social-tn._V396577301_.jpg"
        //  }
        //}
        //];
        //window.kibConfig = 
        //{


        //[
        //{
        //  "type" : "video", 
        //  "mediaObjectId" : "mJRPXDWU3S51F",
        //  "richMediaObjectId" : "",
        //  "preplayImages" : {
        //      "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-01-lg._V401028090_.jpg", 
        //      "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-01-sm._V401028090_.jpg"
        //  },
        //  "html5PreferPosterHeight" : false,
        //  "thumbnailImageUrls" : {
        //      "default" : "http://g-ecx.images-amazon.com/images/G/01/misc/untranslatable-image-id.jpg",
        //      "selected" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-01-tn._V401028090_.jpg"
        //  }
        //}
        //,
        //{
        //  "type" : "video", 
        //  "mediaObjectId" : "m26TT75OS8GNBU",
        //  "richMediaObjectId" : "",
        //  "preplayImages" : {
        //      "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-02-lg._V389678398_.jpg", 
        //      "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-02-sm._V402265591_.jpg"
        //  },
        //  "html5PreferPosterHeight" : false,
        //  "thumbnailImageUrls" : {
        //      "default" : "http://g-ecx.images-amazon.com/images/G/01/misc/untranslatable-image-id.jpg",
        //      "selected" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-02-tn._V389377315_.jpg"
        //  }
        //}
        //,
        //{
        //  "type" : "image",
        //  "imageUrls" : {
        //    "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-03-lg._V400694812_.jpg",
        //    "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-03-sm._V400694812_.jpg",
        //    "rich": {
        //        src: "http://g-ecx.images-amazon.com/images/G/01/misc/untranslatable-image-id.jpg",
        //        width: null,
        //        height: null
        //    }
        //  },
        //  "altText" : "Kindle Paperwhite e-reader",
        //  "thumbnailImageUrls" : {
        //    "default": "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-03-tn._V400694812_.jpg"
        //  }
        //}
        //,
        //{
        //  "type" : "image",
        //  "imageUrls" : {
        //    "L" : "http://g-ecx.images-amazon.com/images/G/01//kindle/dp/2012/KC/KC-slate-04-lg.jpg",
        //    "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-04-sm.jpg",
        //    "rich": {
        //        src: "http://g-ecx.images-amazon.com/images/G/01/misc/untranslatable-image-id.jpg",
        //        width: null,
        //        height: null
        //    }
        //  },
        //  "altText" : "Kindle Paperwhite e-reader",
        //  "thumbnailImageUrls" : {
        //    "default": "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-04-tn._V389394767_.jpg"
        //  }
        //}
        //,
        //{
        //  "type" : "image",
        //  "imageUrls" : {
        //    "L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-05-lg._V389396235_.jpg",
        //    "S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-05-sm.jpg",
        //    "rich": {
        //        src: "http://g-ecx.images-amazon.com/images/G/01/misc/untranslatable-image-id.jpg",
        //        width: null,
        //        height: null
        //    }
        //  },
        //  "altText" : "Kindle Paperwhite 3G: thinner than a pencil",
        //  "thumbnailImageUrls" : {
        //    "default": "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-05-tn._V389377336_.jpg"
        //  }
        //}
        //]

        //1. get json string
        string kibMasJson = "";
        string colorImagesJson = "";

        if (crl.extractSingleStr(@"window\.kibMAs\s*=\s*(\[.+?\])\s*;\s*window\.kibConfig\s*=", respHtml, out kibMasJson, RegexOptions.Singleline))
        {
            //2. json to dict
            Object[] dictList = (Object[])crl.jsonToDict(kibMasJson);

            //3. get ["preplayImages"]["L"]
            imageUrlList = new string[dictList.Length];
            for (int idx = 0; idx < dictList.Length; idx++)
            {
                Dictionary<string, Object> eachImgDict = (Dictionary<string, Object>)dictList[idx];
                Object imgUrlObj = null;
                if (eachImgDict.ContainsKey("preplayImages"))
                {
                    eachImgDict.TryGetValue("preplayImages", out imgUrlObj);
                }
                else if (eachImgDict.ContainsKey("imageUrls"))
                {
                    eachImgDict.TryGetValue("imageUrls", out imgUrlObj);
                }

                if (imgUrlObj != null)
                {
                    //"L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-01-lg._V401028090_.jpg", 
                    //"S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-01-sm._V401028090_.jpg"

                    //"L" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-03-lg._V400694812_.jpg",
                    //"S" : "http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KC/KC-slate-03-sm._V400694812_.jpg",
                    //"rich": {
                    //    src: "http://g-ecx.images-amazon.com/images/G/01/misc/untranslatable-image-id.jpg",
                    //    width: null,
                    //    height: null
                    //}

                    //Type curType = imgUrlObj.GetType();
                    Dictionary<string, Object> imgUrlDict = (Dictionary<string, Object>)imgUrlObj;
                    Object largeImgUrObj = "";
                    if (imgUrlDict.TryGetValue("L", out largeImgUrObj))
                    {
                        //[0]	"http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-01-lg._V395919237_.jpg"
                        //[1]	"http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-02-lg._V389394532_.jpg"
                        //[2]	"http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-03-lg._V389394535_.jpg"
                        //[3]	"http://g-ecx.images-amazon.com/images/G/01//kindle/dp/2012/KT/KT-slate-04-lg.jpg"
                        //[4]	"http://g-ecx.images-amazon.com/images/G/01/kindle/dp/2012/KT/KT-slate-05-lg._V389394532_.jpg"
                        imageUrlList[idx] = largeImgUrObj.ToString();
                    }
                    else
                    {
                        //something wrong
                        //not get all pic
                    }
                }
                else
                {
                    //something wrong
                }
            }
        }
        else if(crl.extractSingleStr(@"var\s+colorImages\s+=\s*{""initial"":(\[{""large"".+?\]}\])}", respHtml, out colorImagesJson))
        {
            //http://www.amazon.com/Pandamimi-Dexule-Fashion-2-Piece-Protector/dp/B008TO8L1Y/ref=sr_1_79?s=wireless&ie=UTF8&qid=1369754594&sr=1-79
            //var colorImages = {"initial":[{"large":"http://ecx.images-amazon.com/images/I/410puGTzNaL.jpg","landing":["http://ecx.images-amazon.com/images/I/410puGTzNaL._SX300_.jpg"],"hiRes":"http://ecx.images-amazon.com/images/I/613THR9iJuL._SL1500_.jpg","thumb":"http://ecx.images-amazon.com/images/I/410puGTzNaL._SS30_.jpg","main":["http://ecx.images-amazon.com/images/I/613THR9iJuL._SX300_.jpg","http://ecx.images-amazon.com/images/I/613THR9iJuL._SX300_.jpg"]},{"large":"http://ecx.images-amazon.com/images/I/519xQtKRA%2BL.jpg","landing":["http://ecx.images-amazon.com/images/I/519xQtKRA%2BL._SY300_.jpg"],"hiRes":"http://ecx.images-amazon.com/images/I/71ozNmnjCOL._SL1000_.jpg","thumb":"http://ecx.images-amazon.com/images/I/519xQtKRA%2BL._SS30_.jpg","main":["http://ecx.images-amazon.com/images/I/71ozNmnjCOL._SY300_.jpg","http://ecx.images-amazon.com/images/I/71ozNmnjCOL._SY300_.jpg"]},{"large":"http://ecx.images-amazon.com/images/I/41UivS6e73L.jpg","landing":["http://ecx.images-amazon.com/images/I/41UivS6e73L._SX300_.jpg"],"hiRes":"http://ecx.images-amazon.com/images/I/61ZV-PN5VnL._SL1500_.jpg","thumb":"http://ecx.images-amazon.com/images/I/41UivS6e73L._SS30_.jpg","main":["http://ecx.images-amazon.com/images/I/61ZV-PN5VnL._SX300_.jpg","http://ecx.images-amazon.com/images/I/61ZV-PN5VnL._SX300_.jpg"]},{"large":"http://ecx.images-amazon.com/images/I/31y%2BwHMFtHL.jpg","landing":["http://ecx.images-amazon.com/images/I/31y%2BwHMFtHL._SX300_.jpg"],"hiRes":"http://ecx.images-amazon.com/images/I/614go2RSKDL._SL1500_.jpg","thumb":"http://ecx.images-amazon.com/images/I/31y%2BwHMFtHL._SS30_.jpg","main":["http://ecx.images-amazon.com/images/I/614go2RSKDL._SX300_.jpg","http://ecx.images-amazon.com/images/I/614go2RSKDL._SX300_.jpg"]},{"large":"http://ecx.images-amazon.com/images/I/319AZIP8xTL.jpg","landing":["http://ecx.images-amazon.com/images/I/319AZIP8xTL._SX300_.jpg"],"hiRes":"http://ecx.images-amazon.com/images/I/61pVLmtnppL._SL1500_.jpg","thumb":"http://ecx.images-amazon.com/images/I/319AZIP8xTL._SS30_.jpg","main":["http://ecx.images-amazon.com/images/I/61pVLmtnppL._SX300_.jpg","http://ecx.images-amazon.com/images/I/61pVLmtnppL._SX300_.jpg"]}]};

            Object[] dictList = (Object[])crl.jsonToDict(colorImagesJson);
            // {"large":"http://ecx.images-amazon.com/images/I/410puGTzNaL.jpg","landing":["http://ecx.images-amazon.com/images/I/410puGTzNaL._SX300_.jpg"],"hiRes":"http://ecx.images-amazon.com/images/I/613THR9iJuL._SL1500_.jpg","thumb":"http://ecx.images-amazon.com/images/I/410puGTzNaL._SS30_.jpg","main":["http://ecx.images-amazon.com/images/I/613THR9iJuL._SX300_.jpg","http://ecx.images-amazon.com/images/I/613THR9iJuL._SX300_.jpg"]}

            imageUrlList = new string[dictList.Length];
            for (int idx = 0; idx < dictList.Length; idx++)
            {
                Object dict = dictList[idx];
                Dictionary<string, Object> imgInfoDict = (Dictionary<string, Object>)dict;
                Object largeUrlObj = null;
                if (imgInfoDict.TryGetValue("large", out largeUrlObj))
                {
                    string largeImgUrl = largeUrlObj.ToString();
                    imageUrlList[idx] = largeImgUrl;
                }
                else
                {
                    //something wrong
                }
            }
        }

        return imageUrlList;
    }


}
