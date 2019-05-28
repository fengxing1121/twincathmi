-- MASTER --
<!DOCTYPE html>
<html lang="en">
<head>
  <title>{{TITLE}}</title>
  <meta charset="utf-8">
  <!-- target-densitydpi=device-dpi -->
  <meta name="viewport" content="{{METAVIEWPORT}}">
  <script>
		TCHMI_ENABLE_DESIGNER_MODE = true;
		TCHMI_ENABLE_DESIGNER_MODE_MASTER = true;
		TCHMI_ENABLE_DESIGNER_MODE_FALLBACK_CONFIG_JSON = {
			"basePath": "{{BASEURL}}",
			"tcHmiServer": {
				"websocketOverwrite": {
					"host": "{{SERVER_HOST}}",
					"port": {{SERVER_PORT}}
				},
				"websocketIntervalTime": 200
			}
		};
  </script>
  {{GLOBAL_JS_INCLUDES}}
</head>
<body>
	{{VIEWLEVEL}}
</body>
</html>
-- /MASTER --

-- SLAVE --
<!DOCTYPE html>
<!--
  Copyright (C) {{YEAR}} {{COMPANYNAME}}    
  For any information visit: {{COMPANYWEBSITE}}

  Deployment
  +++++++++++++++++++++++++++
	Version: {{VERSION}}
	Date: {{DATE}}, Time: {{TIME}}
-->
<html lang="en">
<head>
  <title>{{TITLE}}</title>
  <meta http-equiv="X-UA-Compatible" content="IE=edge" />
  <meta charset="utf-8">
  <!-- target-densitydpi=device-dpi -->
  <meta name="robots" content="noindex, nofollow, noarchive, noimageindex">
  <meta name="google" content="notranslate">
  <meta name="viewport" content="{{METAVIEWPORT}}">
  <link rel="manifest" crossorigin="use-credentials" href="Properties/tchmimanifest.json">
  <link rel="icon" href="Images/Favicon.ico">
  {{GLOBAL_JS_INCLUDES}}
</head>
<body>
	<noscript style="font-size: large; padding: 50px;">
		TwinCAT HMI needs to execute JavaScript Code in the browser. 
		Please enable JavaScript in this browser to use the HMI.
	</noscript>
	{{VIEWLEVEL}}
</body>
</html>
-- /SLAVE --