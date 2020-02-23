<%@ Page Language="C#" AutoEventWireup="true"  %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Aplikace Hlídač státu pro vás není přístupná</title>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <style>
      body              { color: #999999; background-color: #eeeeee; font-family: Consolas, 'Courier New', monospace; }
      a:link, a:visited { color: #666666; }
      a:hover           { color: #000000; }
      h1                { border-bottom: dashed 1px #999999; padding-bottom: .2ex; font-size: x-large; font-weight: normal; }
      .content          { width: 700px; margin: 50px auto; background-color: #ffffff; padding: 3ex; border: solid 1px #999999; }
    </style>
</head>
<body>
    <div class="content">
        <h1>Aplikace Hlídač státu pro vás není přístupná</h1>
        <p>
            Aplikace Hlídač státu pro vás není přístupná, pravděpodobně z důvodu <b>přetěžování</b> či z důvodu opakovaných <b>pokusů o narušení bezpečnosti</b> Hlídače státu.
            Vaše IP adresa <b><% Response.Write(HttpContext.Current.Request.UserHostAddress.ToLower()); %></b> byla zalogována.
        </p>
        <p>
            Pokud si myslíte, že jde o omyl, napište nám.
        </p>
        <p>
            <strong>
                podpora@hlidacstatu.cz
            </strong>
        </p>

        <hr />

        <h1>You are banned from Hlídač státu application.</h1>
        <p>
            The Hlídač státu application is not accessible to you, possibly because of <b>overloading</b> or repeated <b>attempts to violate the security</b> of the application. 
            Your IP address <b><% Response.Write(HttpContext.Current.Request.UserHostAddress.ToLower()); %></b> has been logged in.
        </p>
        <p>
            If you think this is a mistake, please email us.
        </p>
        <p>
            <strong>
                podpora@hlidacstatu.cz
            </strong>
        </p>
    </div>
</body>
</html>
