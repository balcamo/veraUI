const PROXY_CONFIG = [
  {
    //add endpoins to this list
    context: [
      "/api/API",
     
            

    ],
    // TODO : change to server location/url
   // target: "http://localhost:64154",
    target: "http://bigfoot.verawp.local",

        secure: false
    }
]

module.exports = PROXY_CONFIG;
