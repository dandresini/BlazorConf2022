# BlazorConf 2022
## Configurare Blazor Wasm con Azure B2C non è mai stato così semplice!

Spesso la gestione delle identità e degli accessi è un problema e un costo significativo per le applicazioni aziendali, infatti le credenziali degli utenti finali, anche in ottica GDPR, è tutto tranne che una passeggiata: le credenziali vanno conservate in luoghi sicuri, i sistemi **backuppati**, tracciata la loro gestione, gli utenti hanno il diritto di poter cambiare password, gli utenti dimenticano le loro credenziali e quindi si spende tempo per gestione e manutenzione, difficilmente fatturabile.
La gestione e la manutenzione dell'autenticazione è solo un costo aziendale e un problema per la parte legale di cui però non si può fare a meno. 
Inoltre la clientela, con ritmi abbastanza elevati, è abituata ad applicazioni che usiamo tutti i giorni che permettono di autenticarsi ai servizi usando la “nostra” identità pubblica legata ai servizi aziendali come Microsoft 365 o personali come Google, Facebook, Twitter chi più ne ha più ne mette.

La domanda chiave è: perché utilizzare Azure B2C?
**Con Azure AD B2C il costo può essere azzerato perché oltre a cancellare i costi legati alla manutenzione e gestione dell’infrastruttura di autenticazione è un servizio a costo zero sino a 50.000 accessi al mese (per accessi non intendiamo utenti registrati).**

Prima di entrare nel vivo ripassiamo un paio di concetti che talvolta vengono confusi tra di loro; comunque non è chiaro il confine tra un concetto e l'altro. Si tratta dei concetti di autenticazione e autorizzazione.
L'autenticazione è il processo attraverso cui si verifica se un utente è realmente chi dice di essere; in questa definizione si tira in ballo l’entità per indicare che non è necessariamente l'utente, ma può essere anche un'applicazione, quindi qualsiasi elemento che ha una propria identità.

Invece l'autorizzazione è il processo che concede a un'entità il permesso di accedere a una determinata risorsa, che può essere un file oppure un servizio o qualunque altra cosa che offre dei dati o delle funzionalità ad altri.

Questo dovrebbe essere abbastanza chiaro; quello che potrebbe essere meno chiaro ai più è qual è la relazione tra autenticazione e autorizzazione, perché spesso il processo di autenticazione viene implicitamente usato per autorizzare l'accesso a una risorsa. In certi contesti questo può avere senso, perché si autentica un utente è già il fatto stesso di averlo autenticato lo rende autorizzato a effettuare operazioni, però in linea di massima i due concetti sono distinti.

Per chiarire meglio facciamo un esempio della vita reale: 

*una persona acquista un biglietto per un volo in aereo e il suo nome viene scritto sul biglietto stesso; quando il passeggero va all’imbarco,  l'operatore gli chiede un documento perché deve verificare l’identità della persona, ciò che il passeggero sia effettivamente quello scritto sul biglietto. Questa operazione è un’autenticazione. Identificato il passeggero, l’assistente di terra lo autorizza a salire a bordo; tale autorizzazione deriva non tanto dall’identificazione, quanto dal fatto che il passeggero ha un biglietto valido.*

*Se invece si prende il caso di un viaggiatore che compera un biglietto del bus, non c’è identificazione perché il biglietto non è nominativo, ma la condizione per salire a bordo che è abbia il biglietto valido: questa è un’autorizzazione, nel senso che la persona è autorizzata a salire a bordo senza bisogno che sia identificata, ma basta che possegga il biglietto, che rappresenta il mezzo autorizzativo.
Il biglietto non identifica, in questo caso, la persona, ma è solo l’elemento di autorizzazione e prescinde dall’entità che deve essere autorizzata.*

Azure B2C comunica con l'applicazione interponendosi tramite browser in fase di autenticazione quindi in modo totalmente trasparente all'utilizzatore a cui viene proposta una pagina di accesso personalizzabile. Inoltre non è in alcun modo vincolato all’infrastruttura su cui risiede l’applicazione poiché agisce a livello di servizio di autenticazione.

In Azure B2C **con “OpenID Connect” estendiamo il protocollo di autorizzazione OAuth 2.0** per consentirne l'uso come protocollo di autenticazione consentendo di eseguire l'accesso Single Sign-On introducendo cosi il concetto di token ID, che consente al client di verificare l'identità dell'utente e ottenere informazioni del suo profilo di base. 
Come precedentemente indicato poiché estende OAuth 2.0, consente alle applicazioni di acquisire in modo sicuro i token di accesso usati per accedere alle risorse protette tramite un server di autorizzazione. Questo tipo di approccio è consigliabile se si sta creando un'applicazione Web accessibile da browser. 
Inoltre Azure B2C estende ulteriormente l’OpenID Connect per non limitarsi esclusivamente a semplici operazioni di autenticazione e autorizzazione, introducendo il parametro del flusso utente per permettere all'applicazione, ad esempio, la possibilità di iscriversi.

Inizialmente per applicazioni SPA si utilizzava il flusso Implicito dove la caratteristica è che i token (token ID o token di accesso) sono restituiti dall'endpoint /authorize anziché dall'endpoint /token. Il flusso implicito è stato abbandonato da B2C perché attualmente la maggior parte dei browser bloccano i cookie di terze parti, così si è scelto di utilizzare il flusso di “Authorization Code” con l’estensione PKCE [Proof Key for Code Exchange ->Chiave di prova per lo scambio di codice] per mantenere gli utenti connessi anche quando i cookie di terze parti vengono bloccati rendendo anche il flusso più sicuro limitando così le probabilità che l'Authorization Token venga intercettato. L’estensione non introduce alcuna nuova richiesta, ma prevede solamente l'uso di ulteriori parametri nelle richieste standard e il suo funzionamento è abbastanza semplice:
1.	l'applicazione genera dapprima un identificativo casuale, il “code verifier”, al quale viene applicata una funzione di hash per generare il code challenge.
2.	Il code challenge è incluso nella richiesta di generazione dell'Authorization Token, assieme agli altri parametri visti in precedenza.
3.	L'Authorization Server a questo punto genera l'Authorization Token, lo associa al code challenge e lo "restituisce" al client.
4.	L'applicazione invia la richiesta di generazione di Access Token, includendo l'Authorization Token e il "code verifier".
5.	L'Authorization Server, prima di generare il token, verifica che il "code verifier" corrisponda al "code challenge" inviato in precedenza, applicando la funzione di hash.
6.	Nel caso in cui non corrisponda, l'Authorization Server restituisce una condizione di errore, altimenti genera e restituisce l'Access Token.

### Esempio di chiamata all'endpoint authorize (non vi preoccupate fa tutto Azure B2C)

    https://blazorconfita.b2clogin.com/blazorconfita.onmicrosoft.com/b2c_1_susi/oauth2/v2.0/authorize?
    client_id=4b80c7c5-3a11-4102-a48d-e8986dbfc8f8
    &scope=api.write api.read openid profile offline_access
    &redirect_uri=https%3A%2F%2Flocalhost%3A7022%2Fauthentication%2Flogin-callback
    &client-request-id=72668def-8ace-4030-952b-e24eb9ab715b&
    response_mode=fragment
    &response_type=code
    **&code_challenge=a4AIjJUHrCOMQc5U-LVDlfcenNqNOwi12WgfV3oAijM**
    **&code_challenge_method=S256**
   
### Esempio di chiamata all'endpoint token  (non vi preoccupate fa tutto Azure B2C)

    https://blazorconfita.b2clogin.com/blazorconfita.onmicrosoft.com/b2c_1_susi/oauth2/v2.0/token
    client_id: 4b80c7c5-3a11-4102-a48d-e8986dbfc8f8
    redirect_uri: https://localhost:7022/authentication/login-callback
    scope: https://blazorconfITA.onmicrosoft.com/backendapi/api.write openid profile offline_access
    code: yJraWQiOiJjcGltY29yZV8wOTI1MjAxNSIsInZlciI6IjEuMCIsInppcCI6IkRlZmxhdGUiLCJzZXIiOiIxLjAifQ..LLEcvf-
    **code_verifier: rts1LnHpDUVNikR_jN4dSwyyfMbIPCSoXi8EgF7LgWQ**
    grant_type: authorization_code
    

## Prima di eseguire il codice

Assicuratevi che nella parte Properties->launchSettings.json del progetto **BlazorConf2022.Server** e **BlazorConf2022.Client** sia impostata la porta 7064 del localhost.

    "profiles": {
        "BlazorConf2022": {
        "commandName": "Project",
        "dotnetRunMessages": true,
        "launchBrowser": true,
        "inspectUri": "{wsProtocol}://{url.hostname}:{url.port}/_framework/debug/ws-proxy?browser={browserInspectUri}",
        "applicationUrl": "https://localhost:7064;http://localhost:5022",
        "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }

## Configurazione dell'Identity Provider su Twitter

Durante la presentazione non sono riuscito ad attivare l'IDP di Twitter perchè come avevo spiegato in precedenza, ma poi non ho messo in pratica :-), nella sezione App del portale developer di twitter <https://developer.twitter.com/> si deve inserire in minuscolo l'intera URI della callback:

    https://blazorconfita.b2clogin.com/blazorconfita.onmicrosoft.com/b2c_1_susi/oauth1/authresp

e non 

    https://blazorconfITA.b2clogin.com/blazorconfITA.onmicrosoft.com/b2c_1_susi/oauth1/authresp
    
