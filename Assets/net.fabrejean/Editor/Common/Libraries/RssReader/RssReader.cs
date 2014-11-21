#region Using

using System;
using System.Xml;
using System.Collections.ObjectModel;
using System.Web;

#endregion

/// <summary>
/// Parses remote RSS 2.0 feeds.
/// </summary>
[Serializable]
public class RssReader : IDisposable
{

  #region Constructors

  public RssReader()
  { }

  public RssReader(string feedUrl)
  {
    _FeedUrl = feedUrl;
  }

  #endregion

  #region Properties

  private string _FeedUrl;
  /// <summary>
  /// Gets or sets the URL of the RSS feed to parse.
  /// </summary>
  public string FeedUrl
  {
    get { return _FeedUrl; }
    set { _FeedUrl = value; }
  }

  private Collection<RssItem> _Items = new Collection<RssItem>();
  /// <summary>
  /// Gets all the items in the RSS feed.
  /// </summary>
  public Collection<RssItem> Items
  {
    get { return _Items; }
  }

  private string _Title;
  /// <summary>
  /// Gets the title of the RSS feed.
  /// </summary>
  public string Title
  {
    get { return _Title; }
  }

  private string _Description;
  /// <summary>
  /// Gets the description of the RSS feed.
  /// </summary>
  public string Description
  {
    get { return _Description; }
  }

  private DateTime _LastUpdated;
  /// <summary>
  /// Gets the date and time of the retrievel and
  /// parsing of the remote RSS feed.
  /// </summary>
  public DateTime LastUpdated
  {
    get { return _LastUpdated; }
  }

 // private TimeSpan _UpdateFrequenzy;
  /// <summary>
  /// Gets the time before the feed get's silently updated.
  /// Is TimeSpan.Zero unless the CreateAndCache method has been used.
  /// </summary>
 // public TimeSpan UpdateFrequenzy
  //{
   // get { return _UpdateFrequenzy; }
  //}

  #endregion

  #region Methods

	

	public static RssReader Create(string feedUrl)
	{
		RssReader reader = new RssReader(feedUrl);
		reader.Execute();
		return reader;
	}

	/*
  /// <summary>
  /// Creates an RssReader instance from the specified URL
  /// and inserts it into the cache. When it expires from the cache, 
  /// it automatically retrieves the remote RSS feed and inserts it 
  /// to the cache again.
  /// </summary>
  /// <param name="feedUrl">The URI of the RSS feed.</param>
  /// <param name="updateFrequenzy">The time before it should update it self.</param>
  /// <returns>An instance of the RssReader class.</returns>
  public static RssReader CreateAndCache(string feedUrl, TimeSpan updateFrequenzy)
  {
    if (HttpRuntime.Cache["RssReader_" + feedUrl] == null)
    {
      RssReader reader = new RssReader(feedUrl);
      reader.Execute();
      reader._UpdateFrequenzy = updateFrequenzy;
      HttpRuntime.Cache.Add("RssReader_" + feedUrl, reader, null, DateTime.Now.Add(updateFrequenzy), Cache.NoSlidingExpiration, CacheItemPriority.Normal, RefreshCache);
    }

    return (RssReader)HttpContext.Current.Cache["RssReader_" + feedUrl];
  }
*/

	/*
  /// <summary>
  /// Retrieves the remote RSS feed and inserts it into the cache
  /// when it has expired.
  /// </summary>
  private static void RefreshCache(string key, object item, CacheItemRemovedReason reason)
  {
    if (reason != CacheItemRemovedReason.Removed)
    {
      string feedUrl = key.Replace("RssReader_", String.Empty);
      RssReader reader = new RssReader(feedUrl);
      reader.Execute();
      reader._UpdateFrequenzy = ((RssReader)item).UpdateFrequenzy;
      HttpRuntime.Cache.Add("RssReader_" + feedUrl, reader, null, DateTime.Now.Add(reader.UpdateFrequenzy), Cache.NoSlidingExpiration, CacheItemPriority.Normal, RefreshCache);
    }
  }
*/

  /// <summary>
  /// Retrieves the remote RSS feed and parses it.
  /// </summary>
  /// <exception cref="System.Net.WebException" />
  public Collection<RssItem> Execute()
  {
    if (String.IsNullOrEmpty(FeedUrl))
      throw new ArgumentException("The feed url must be set");

    using (XmlReader reader = XmlReader.Create(FeedUrl))
    {
      XmlDocument doc = new XmlDocument();
      doc.Load(reader);

      ParseElement(doc.SelectSingleNode("//channel"), "title", ref _Title);
      ParseElement(doc.SelectSingleNode("//channel"), "description", ref _Description);
      ParseItems(doc);

      _LastUpdated = DateTime.Now;
      
      return _Items;
    }
  }

  /// <summary>
  /// Parses the xml document in order to retrieve the RSS items.
  /// </summary>
  private void ParseItems(XmlDocument doc)
  {
    _Items.Clear();
    XmlNodeList nodes = doc.SelectNodes("rss/channel/item");

    foreach (XmlNode node in nodes)
    {
      RssItem item = new RssItem();
      ParseElement(node, "title", ref item.Title);
      ParseElement(node, "description", ref item.Description);
      ParseElement(node, "link", ref item.Link);

      string date = null;
      ParseElement(node, "pubDate", ref date);
      DateTime.TryParse(date, out item.Date);

      _Items.Add(item);
    }
  }

  /// <summary>
  /// Parses the XmlNode with the specified XPath query 
  /// and assigns the value to the property parameter.
  /// </summary>
  private void ParseElement(XmlNode parent, string xPath, ref string property)
  {
    XmlNode node = parent.SelectSingleNode(xPath);
    if (node != null)
      property = node.InnerText;
    else
      property = "Unresolvable";
  }

  #endregion

  #region IDisposable Members

  private bool _IsDisposed;

  /// <summary>
  /// Performs the disposal.
  /// </summary>
  private void Dispose(bool disposing)
  {
    if (disposing && !_IsDisposed)
    {
      _Items.Clear();
      _FeedUrl = null;
      _Title = null;
      _Description = null;
    }

    _IsDisposed = true;
  }

  /// <summary>
  /// Releases the object to the garbage collector
  /// </summary>
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  #endregion

}

#region RssItem struct

/// <summary>
/// Represents a RSS feed item.
/// </summary>
[Serializable]
public struct RssItem
{
  /// <summary>
  /// The publishing date.
  /// </summary>
  public DateTime Date;

  /// <summary>
  /// The title of the item.
  /// </summary>
  public string Title;

  /// <summary>
  /// A description of the content or the content itself.
  /// </summary>
  public string Description;

  /// <summary>
  /// The link to the webpage where the item was published.
  /// </summary>
  public string Link;
}

#endregion