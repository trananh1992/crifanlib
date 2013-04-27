/**
 * [File]
 * crifanLib.java
 * 
 * [Function]
 * 1. implement crifan's common functions
 * 
 * [Version]
 * v1.0
 * 2013-04-27
 * 
 * [History]
 * 1. add http related func and regex related func
 */

package crifan.com;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.CookieManager;
import java.net.CookiePolicy;
import java.net.HttpCookie;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.NameValuePair;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.CookieStore;
//import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.cookie.Cookie;
import org.apache.http.impl.client.BasicCookieStore;
import org.apache.http.impl.client.DefaultHttpClient;
//import org.apache.http.impl.cookie.BasicClientCookie;
import org.apache.http.params.HttpParams;

import org.apache.http.protocol.BasicHttpContext;
import org.apache.http.protocol.HttpContext;
import org.apache.http.client.params.ClientPNames;
import org.apache.http.client.protocol.ClientContext;

import org.apache.http.util.EntityUtils;

//import crifan.com.downloadsongtastemusic.R;

//import android.os.Environment;
//import android.widget.EditText;
//import android.app.Activity;

public class crifanLib {
	CookieStore localCookies = null;
	
	public crifanLib()
	{
		localCookies = new BasicCookieStore();
	}

    /** Get response from url, headerDict, postDict */
    public HttpResponse getUrlResponse(String url,
    							HttpParams headerParams,
								List<NameValuePair> postDict)
    {
    	// init
    	HttpResponse response = null;
    	HttpUriRequest request = null;
    	DefaultHttpClient httpClient = new DefaultHttpClient();
    	
    	//default enable auto redirect
    	headerParams.setBooleanParameter(ClientPNames.HANDLE_REDIRECTS, Boolean.TRUE);

    	if(postDict != null)
    	{
    		HttpPost postReq = new HttpPost(url);
    		
    		try{
    			HttpEntity postBodyEnt = new UrlEncodedFormEntity(postDict);
    			postReq.setEntity(postBodyEnt);
    		}
    		catch(Exception e){
    			
    		}

    		request = postReq;
    	}
    	else
    	{
        	HttpGet getReq = new HttpGet(url);
        	
        	request = getReq;
    	}

    	if(headerParams != null)
    	{
    		request.setParams(headerParams);
    	}

		try{			
			HttpContext localContext = new BasicHttpContext();
			localContext.setAttribute(ClientContext.COOKIE_STORE, localCookies);
			response = httpClient.execute(request, localContext);
			List<Cookie> respCookieList = localCookies.getCookies();
			System.out.println("Cookies for " + url);
			for(Cookie ck : respCookieList)
			{
				System.out.println(ck);
			}
			
        } catch (ClientProtocolException cpe) {
            // TODO Auto-generated catch block
        	cpe.printStackTrace();    
        } catch (IOException ioe) {
            // TODO Auto-generated catch block
        	ioe.printStackTrace();
        }
		
    	return response;
    }

    /** Get response html from url, headerDict, html charset, postDict */
    public String getUrlRespHtml(String url,
    							HttpParams headerParams, 
								String htmlCharset, 
								List<NameValuePair> postDict)
    {
    	// init
    	String respHtml = "";
    	String defaultCharset = "UTF-8";
    	// check para
    	if( (null == htmlCharset) || (htmlCharset != null ) && (htmlCharset == ""))
    	{
    		htmlCharset = defaultCharset;
    	}
    	
    	//init 
    	//HttpClient httpClient = new DefaultHttpClient();
    	//DefaultHttpClient httpClient = new DefaultHttpClient();
    	//HttpUriRequest request;

		try{
			
			HttpResponse response = getUrlResponse(url, headerParams, postDict);
			
			if(response.getStatusLine().getStatusCode()==HttpStatus.SC_OK){
				HttpEntity respEnt = response.getEntity();
				
				respHtml = EntityUtils.toString(respEnt, htmlCharset);
	        }
	        
        } catch (ClientProtocolException cpe) {
            // TODO Auto-generated catch block
        	cpe.printStackTrace();    
        } catch (IOException ioe) {
            // TODO Auto-generated catch block
        	ioe.printStackTrace();
        }
		
    	return respHtml;
    }

    /** Get response html from url and designated html charset */
    public String getUrlRespHtml(String url, String htmlCharset)
    {
    	return getUrlRespHtml(url, null, htmlCharset, null);
    }
    
    /** Get response html from url, use default UTF-8 html charset */
    public String getUrlRespHtml(String url)
    {
    	String defaulCharset = "UTF-8";
    	return getUrlRespHtml(url, defaulCharset);
    }
    
    public interface UpdateProgressCallback
    {
        // This is just a regular method so it can return something or
        // take arguments if you like.
        public void updateProgress(long currentSize, long totalSize);
    }

    /**
     *  download file from file url
     * eg:
     * http://m5.songtaste.com/201212211424/2e8a8a85d93f56370d7fd96b5dc6ff23/5/5c/5cf23a97cef6fad6a464eb506c409dbd.mp3
     * with header: Referer=http://songtaste.com/
     *  */
    public Boolean downlodFile(String url, File fullFilename, HttpParams headerParams, UpdateProgressCallback updateProgressCallbak)
    {
    	Boolean downloadOk = Boolean.FALSE;
    	
    	HttpResponse response = getUrlResponse(url, headerParams, null);

		if(response.getStatusLine().getStatusCode()==HttpStatus.SC_OK){
			
			HttpEntity respEnt = response.getEntity();
			
			System.out.println("isChunked" + respEnt.isChunked());
			System.out.println("Streaming" + respEnt.isStreaming());
			
			Boolean isStream = respEnt.isStreaming();
			if(isStream){
				try {
					InputStream fileInStream = respEnt.getContent();
					
					FileOutputStream fileOutStream = new FileOutputStream(fullFilename);
					
					long totalSize = respEnt.getContentLength();
					byte[] tmpBuf = new byte[8192];
					int bufLen = 0;
					long downloadedSize = 0;
					while( (bufLen = fileInStream.read(tmpBuf)) > 0 ) {
						fileOutStream.write(tmpBuf,0, bufLen);
						downloadedSize += bufLen;
						
						//System.out.println(Long.toString((downloadedSize/totalSize)*100)+"%");
						//System.out.println(Long.toString((downloadedSize*100)/totalSize)+"%");
						updateProgressCallbak.updateProgress(downloadedSize, totalSize);
					}
					fileOutStream.close();
					downloadOk = Boolean.TRUE;
				} catch (IllegalStateException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (IOException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
        }

		return downloadOk;
    }

    /**
     *  none header version of downlodFile
     *  */
    public String downlodFile(String url, String fullFilename)
    {
    	return getUrlRespHtml(url, null, null, null);
    }
    
    /** Extract single string from input whole string
     *	Note:
     * 1. input pattern should include one group, like 'xxx(xxx)xxx'
     * 2. output is in extractedStr
     *  */
    public Boolean extractSingleStr(String pattern, String extractFrom, int flags, StringBuilder extractedStr)
    {
    	Pattern strP = Pattern.compile(pattern, flags);
    	Matcher foundStr = strP.matcher(extractFrom);
    	Boolean found = foundStr.find();
    	if(found)
    	{
    		extractedStr.append(foundStr.group(1));
    	}
    	return found;
    }

    /**
     * None pattern version of  extractSingleStr
     * */
    public Boolean extractSingleStr(String pattern, String extractFrom, StringBuilder extractedStr)
    {
    	return extractSingleStr(pattern, extractFrom, 0, extractedStr);
    }

}
