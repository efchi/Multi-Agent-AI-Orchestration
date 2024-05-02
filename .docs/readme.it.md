
# Multi-Agent AI Orchestration

Questo documento è disponibile in [inglese](readme.en.md) e in [italiano](readme.it.md).

## The most expensive way to sum a list of numbers

In questo repository presento un piccolo software sviluppato nel corso del mio periodo sabbatico, durante il quale ho deciso di sperimentare con le possibilità offerte dall'AI in modo pratico, costruendo un prototipo dimostrativo. 

Il progetto consiste in un **modello di organizzazione basato su agenti AI che cooperano fra loro** per raggiungere un obiettivo comune, implementato in linguaggio C# .NET attraverso un prototipo funzionante.

Questa architettura non è una novità - esistono già sistemi analoghi (come [Microsoft AutoGen](https://microsoft.github.io/autogen/)) consolidati o in corso di sviluppo. Qui propongo un'implementazione semplice, *from-scratch*, dei principi fondamentali che presumibilmente andranno a costituire la prossima generazione dell'ingegneria del software. 

Il modello proposto è un **esperimento concettuale** che potrebbe fornire un'anteprima di come saranno strutturate le aziende del futuro: gruppi di agenti AI specializzati e addestrati per compiti specifici, che comunicano fra loro in modo asincrono e che condividono informazione in modo protetto, trasformando dati ed eseguendo azioni. Riducendo notevolmente o azzerando del tutto la necessità di coinvolgere esseri umani all'interno dei processi aziendali (alla fine di questo documento è possibile leggere un [commento personale](#comment) sull'argomento).

Il codice sorgente è liberamente consultabile e utilizzabile, pubblicato in questo repository GitHub e rilasciato sotto licenza MIT. Per approfondimenti sul tema si rimanda a [Agent-Oriented Programming](https://en.wikipedia.org/wiki/Agent-oriented_programming) e [Actor Model](https://en.wikipedia.org/wiki/Actor_model).



## Premesse

Ispirato dai recenti sviluppi nel campo dell'intelligenza artificiale, ho provato a immaginare come funzioneranno le aziende del futuro, e come l'AI potrebbe trasformare le aziende del presente per proiettarle nel futuro.

In senso astratto, ogni azienda è un'**organizzazione**. Ogni organizzazione è composta da entità più o meno senzienti. Chiamiamo **agenti** queste entità. Le organizzazioni del futuro saranno composte da agenti di 3 tipologie: agenti umani, agenti AI e agenti *hard-coded*, in ordine di costo.

- Gli **agenti umani** (1) saranno molto intelligenti, ma costosi e inefficienti. Saranno convolti nei processi aziendali solo per compiti di supervisione, validazione e accettazione dei risultati proposti dalle AI.
- Gli **agenti AI** (2) saranno meno intelligenti degli umani ma più competenti, flessibili e meno costosi. Saranno utilizzati per tutti quei compiti in cui un livello minimo di intelligenza è sufficiente per sostituire un operatore umano in modo accettabile, in base a criteri di ottimizzazione dei costi e dei risultati.
- Gli **agenti hard-coded** (3) saranno i meno costosi e più efficienti, e assolutamente stupidi. Sono gli attuali *programmi*. Saranno utilizzati per eseguire tutti quei compiti perfettamente algoritmici per i quali non è necessario possedere un livello minimo di intelligenza non-meccanica.

Nelle organizzazioni di oggi regnano gli agenti di tipo 1 e 3. 

I programmi svolgono quei compiti prestabiliti e ripetitivi che necessitano di procedure esatte e input / output ben definiti. Gli umani fanno tutto il resto: la risoluzione di problemi sotto condizioni non predeterminate e incerte, la manipolazione di dati non strutturati, la cooperazione in team, la creazione di procedure e soluzioni riutilizzabili, e così via.

Le organizzazioni del futuro vedranno inserirsi gli agenti di tipo 2 come figura intermedia: non troppo intelligenti, non troppo stupidi, non troppo costosi, efficienti *abbastanza*. In pratica, il lavoratore perfetto. Le AI domineranno le organizzazioni del futuro.



## L'Esperimento

Una delle applicazioni pratiche in cui ChatGPT ha spopolato sin da subito fra gli utenti è quella della simulazione di  **giochi di ruolo**.

Gli si chiede di impersonare qualcuno o qualcosa e l'AI, con massima disponibilità e senza cenni di protesta, imita il soggetto proposto mimandone il linguaggio e il comportamento. Con questo approccio, che trova la sua massima espressione nel cosiddetto *prompt engineering*, è possibile far diventare ChatGPT un professionista a nostro piacimento e nostra piena disposizione. Scrittore, traduttore, sviluppatore di software, artista o illustratore, analista finanziario o CEO di una società - non c'è limite al tipo di personaggio, o **ruolo**, da fargli interpretare. 

Immaginiamo di creare una nostra organizzazione composta da agenti AI che giocano, tutti insieme ma con ruoli diversi, allo stesso gioco. Diamo ad ogni agente uno **script** (un copione) da seguire, specificando le regole del gioco.

    We are playing a role-playing game. 
    Here are the general rules of role-playing games:

    - There are organizations.
    - An organization has one or more agents.
    - An agent of an organization has a role, and can only have one role.
    - An organization can have multiple agents with the same role.
    - Agents of an organization cooperate to reach a common goal.
	  They work in parallel by exchanging information asynchronously using message queues.
    - Each role has a message queue that contains input messages for agents of that role.
    - An agent can send messages to other agents writing to the proper message queue.
    - An orchestrator decides which messages to dequeue and assign to agents.
    - Messages can be grouped into jobs, which represent a distributed unit of work.

Il testo appena mostrato contiene le **meta-regole** del gioco. Queste regole istruiscono un qualsiasi agente sul contesto di esecuzione di un generico gioco. 

Specificano che la nostra organizzazione è composta da agenti che si scambiano messaggi in modo asincrono tramite un [sistema di code](https://en.wikipedia.org/wiki/Message_queue). Specificano inoltre che ogni agente impersona un ruolo specifico, e che la comunicazione fra agenti è mediata da un **orchestrator** - un sistema di runtime che smista i messaggi e supervisiona l'accesso alle risorse in modo simile a quanto farebbe un sistema operativo.

Entriamo adesso nel vivo dell'esperimento. Supponiamo che l'obiettivo della nostra organizzazione sia di sommare liste di numeri ricevute in input dall'utente, estendendo il nostro script con regole più concrete.

    Our game is called "The most expensive way to sum a list of numbers".
    Here are the game rules:

    - You are an agent working for the organization NaiveSummers.
    - The goal of the organization NaiveSummers is to sum a list of numbers.
    - Agents of NaiveSummers are organized into the following roles:
	    - Role A: an agent that starts the operations.
	    - Role B: an agent that receives a list of numbers to sum and forwards each single numbers to sum.
	    - Role C: an agent that receives a single number to sum and sums it with the intermediate result, starting from 0.
	    - Role D: an agent that receives final result and signals the completion of the operation.

    For example, we call Agent X an agent of role X.
    - Agent A starts the operations by sending a list of numbers to Agent B.
    - Agent B forwards each number to Agent C. When all numbers are processed, Agent B sends the final result to Agent D and closes the operations.
    - Agent C sums the current number with the intermediate result and notifies back to Agent B when an intermediate sum is done.
    - Agent D signals the completion of the operation, showing the final result.

La porzione di testo appena mostrata è il **prologo** del nostro script. Questa sezione specifica le regole di gioco vere e proprie: il nome dell'organizzazione, l'obiettivo da raggiungere, i ruoli da impersonare e le rispettive azioni da compiere per eseguire correttamente la propria parte. Il prologo viene condiviso a tutti gli agenti partecipanti e rappresenta una visione d'insieme delle operazioni.

Per sommare una lista di numeri, la nostra organizzazione è composta da 4 ruoli. Ogni ruolo ha una funzione specifica all'interno del flusso di lavoro. Nel nostro esperimento avremo 4 agenti che impersonano 4 ruoli.

Immaginiamo un ruolo come un'entità associata a una coda di messaggi. Possono esistere più agenti per ogni ruolo, ma un agente possiede uno e un solo ruolo. Nel nostro ambiente, quello vagamente descritto dalle delle meta-regole, un agente è istanza di un ruolo e rappresenta un'unità di forza-lavoro dotata di una propria memoria interna (proprio come l'istanza di una conversazione con un LLM).

Sotto un punto di vista strettamente tecnico un agente è assimilabile a un *thread*, un'unità di elaborazione asincrona che comunica con i suoi pari leggendo e scrivendo messaggi nelle code opportune. Un ruolo specifica quindi il tipo di comportamento che tale unità di lavoro deve manifestare.

I ruoli e il **workflow** del nostro esperimento sono descritti in modo più efficace dal diagramma seguente.

![Agents Workflow Diagram](workflow-diagram.jpg)

Supponiamo che i nostri agenti AI non dispongano di una buona memoria a breve termine, o che comunque la loro memoria sia inaffidabile e soggetta ad errori, in modo simile a quanto avviene per gli esseri umani. 

Gli agenti avranno bisogno di accedere a una memoria condivisa per memorizzare i dati delle operazioni in corso. Chiamiamo **stato** questa memoria. L'accesso allo stato sarà mediato dal runtime - l'orchestrator, sottoforma di macchina virtuale - che avrà il compito di controllare l'accesso ai dati e di fornire le primitive di comunicazione fra agenti per realizzare lo scambio di messaggi.

Vediamo il workflow cooperativo nel dettaglio:

- **Ruolo A**. L'agente A riceve in input una lista di numeri da sommare. Se la lista è vuota, l'agente A inoltra il risultato (zero) all'agente D. Se la lista contiene un solo elemento, l'agente A inoltra quell'elemento all'agente D come risultato. Se invece la lista contiene più di un numero, l'agente A apre uno stato condiviso e lo inizializza, memorizzando la lista di numeri e il risultato intermedio (zero), quindi invoca l'agente B inviandogli un messaggio.

- **Ruolo B**. L'agente B riceve in input un messaggio da A per iniziare le operazioni di somma. Se la lista contiene almeno un numero, l'agente B rimuove l'ultimo numero dalla lista (*pop*) e lo inoltra all'agente C, aggiornando lo stato. Se invece la lista è vuota, l'agente B inoltra il risultato intermedio all'agente D e chiude lo stato. 

- **Ruolo C**. L'agente C riceve in input un numero da sommare. L'agente C somma il numero appena ricevuto al risultato intermedio, aggiornando lo stato, quindi notifica il completamento dell'operazione all'agente B.

- **Ruolo D**. L'agente D riceve in input il risultato finale e segnala all'utente il completamento delle operazioni.

I nostri agenti accetteranno in input e restituiranno in output messaggi esclusivamente in **formato JSON**. L'input di un agente sarà un messaggio proveniente da un altro agente corredato dal contesto, cioè dalla memoria contenente lo stato delle operazioni. L'output di un agente non sarà, banalmente, l'input da inoltrare al prossimo agente, ma una struttura contenente una serie di **istruzioni** che il runtime dovrà interpretare ed eseguire per continuare il workflow. Tali istruzioni richiamano primitive per il flusso di controllo e per la manipolazione dei dati (come avviene in qualsiasi macchina astratta).

Per svolgere correttamente il proprio compito, ogni ruolo possiede uno **script di ruolo** specifico che ne descrive in dettaglio il comportamento da seguire. Mostreremo gli script specifici per ogni ruolo nella sezione successiva. Per adesso, chiudiamo questa sezione mostrando l'**epilogo**, cioè la sezione finale del nostro script.

	You'll begin to receive some input messages to process. 
    Please wait for the first message. When responding to messages, you must not write anything 
    other JSON code in the format described above.
    Good game!



## Specifica dei Ruoli

Vediamo adesso in concreto gli script relativi ad ogni ruolo. Questi script, a differenza del prologo, sono molto dettagliati e ricordano la struttura di uno pseudo-codice, pur essendo scritti in linguaggio naturale.


### Ruolo A

    You are an agent of Role A.

    As a role A agent, you will be provided with an input message in JSON format.
    The input message has the following structure:

	    { "list_of_items": LIST_OF_ITEMS }

    Where LIST_OF_ITEMS is a list of numbers.

    Let LAST_ITEM be the last item of LIST_OF_ITEMS (if LIST_OF_ITEMS is not empty).
    If and only if LIST_OF_ITEMS is empty, you must respond with a JSON message with the following structure:

	    { "instructions": [ ["forward", "D", { "result": 0 }] ] }

    Otherwise, if and only if LIST_OF_ITEMS has only one item, you must respond with a JSON message with the following structure:

	    { "instructions": [ ["forward", "D", { "result": LAST_ITEM }] ] }

    Otherwise, if and only if LIST_OF_ITEMS has more than one item, you must respond with a JSON message with the following structure:
	
	    { "instructions": [ ["open"], ["set", "list_of_items", LIST_OF_ITEMS], ["set", "intermediate_result", 0], ["forward", "B"] ] }


### Ruolo B

    You are an agent of Role B.

    As a role B agent, you will be provided with an input message in JSON format.
    The input message has the following structure:

	    { "state": { "list_of_items": LIST_OF_ITEMS, "intermediate_result": INTERMEDIATE_RESULT } }

    Where LIST_OF_ITEMS is a list of numbers, and INTERMEDIATE_RESULT is a number.
    Other fields may be present inside the "state" object, but they are not relevant for this task and you must ignore them.

    Let LAST_ITEM be the last item of LIST_OF_ITEMS (if LIST_OF_ITEMS is not empty).

    If and only if LIST_OF_ITEMS is empty, you must respond with a JSON message with the following structure:

	    { "instructions": [ ["close"], ["forward", "D", { "result": INTERMEDIATE_RESULT }] ] }

    Otherwise, if and only if LIST_OF_ITEMS is not empty, you must respond with a JSON message with the following structure:
	
	    { "instructions": [ ["pop", "list_of_items"], ["forward", "C", { "item": LAST_ITEM }] ] }


### Ruolo C

    You are an agent of Role C.

    As a role C agent, you will be provided with an input message in JSON format.
    The input message has the following structure:

	    { "state": { "intermediate_result": INTERMEDIATE_RESULT }, "data": { "item": CURRENT_ITEM } } }

    Where INTERMEDIATE_RESULT is a number and CURENT_ITEM is a number.
    Other fields may be present inside the "state" object, but they are not relevant for this task and you must ignore them.

    Your task is to sum the intermediate result with the current item.
    Let SUM be the result of the sum operation.

    You must respond with a JSON message with the following structure:

	    { "instructions": [ ["set", "intermediate_result", SUM], ["forward", "B"] ] }


### Ruolo D

    You are an agent of Role D.

    As a role D agent, you will be provided with an input message in JSON format.
    The input message has the following structure:

	    { "data": { "result": RESULT } }

    Where RESULT is a number.

    Your task is to signal the completion of the sum workflow.

    You must respond with a JSON message with the following structure:

	    { "instructions": [ ["complete", RESULT] ] }



### Funzione Equivalente

Gli script appena proposti non fanno altro che realizzare, seguendo un paradigma asincrono basato sullo scambio di messaggi, il seguente pseudo-codice.


    agent A(input) => 
        if (input.length == 0)
            forward 0 to D
        else if (input.length == 1) 
            forward input.first to D
	    else
            state.open()
            state.set("list_of_items", input)
            state.set("intermediate_result", 0)
            forward to B

    agent B => 
	    if (state.get("list_of_items").length == 0)
            intermediate_result = state.get("intermediate_result")
            state.close()
            forward intermediate_result to D
	    else
            list_of_items = state.get("list_of_items")
            last_item = list_of_items.pop()
            forward last_item to C

    agent C(current_item) =>
        intermediate_result = state.get("intermediate_result")
	    sum = intermediate_result + current_item
	    state.set("intermediate_result", sum)
	    forward to B

    agent D(result) =>
	    print(result)

    # Start Workflow
    Sum([1, 2, 3, 42])


Che non è altro che un modo asincrono, distribuito e molto complicato per realizzare, più o meno, la seguente funzione espressa in pseudo-codice.


    list_of_items = [1, 2, 3, 42]
    intermediate_result = 0

    function Sum(input) =>
        if (input.length == 0)                                  # Agent A
            print 0                                             # Agent D
        else if (input.length == 1)                             # Agent A
            print input.first                                   # Agent D
	    else
            list_of_items = input                               # Agent A
            intermediate_result = 0                             # Agent A
            do
                var current_item = list_of_items.pop()          # Agent B
                var sum = intermediate_result + current_item    # Agent C
                intermediate_result = sum                       # Agent C
            while (list.length > 0)                             # Agent B
            print intermediate_result                           # Agent D

    # Start Workflow
    Sum([1, 2, 3, 42])



## L'Implementazione

Dopo aver progettato l'esperimento concettuale e aver specificato le regole del gioco sottoforma di script, ho proceduto all'implementazione concreta del prototipo. Non vale la pena di addentrarsi nei dettagli del codice sorgente che di per sé ha una struttura semplice e senza pretese. In questa sezione mi limito a fornire una panoramica di alto livello sulle funzionalità implementate e sul processo di sviluppo.

La cartella `Architecture` all'interno della soluzione contiene le classi principali. `Agent`, `Message`, `Role` e `Script` non necessitano di ulteriori spiegazioni. Le altre classi più importanti sono:
- `Process` che rappresenta il contesto di esecuzione di un workflow,
- `Queue` che implementa una coda di messaggi asincrona,
- `Reaction` che rappresenta l'output di un agente sottoforma di lista di istruzioni,
- `Machine` che implementa una VM per l'esecuzione delle istruzioni prodotte dagli agenti.

La classe `Machine` implementa, in particolare, le primitive essenziali per realizzare correttamente il workflow proposto in questo esperimento:
- `open` per aprire un nuovo stato,
- `close` per chiudere uno stato,
- `set` per inserire o sovrascrivere un valore nello stato,
- `pop` per rimuovere l'ultimo elemento da una lista memorizzata nello stato,
- `forward` per inoltrare un messaggio a un agente, con relativi parametri.

In una prima fase i 4 ruoli sono stati implementati in modo hard-coded, scrivendo delle procedure apposite che utilizzano la libreria `Newtonsoft.JSON` per il parsing dei messaggi. È possibile trovare queste procedure all'interno della classe `MockLambdas`.

Dopo aver verificato la correttezza dell'implementazione *mock*, i ruoli sono stati finalmente implementati tramite AI, utilizzando le API di OpenAI e la libreria `Azure.AI.OpenAI` come client. Il modello LLM utilizzato è `gpt-3.5-turbo` con opzione JSON-mode abilitata e parametro `temperature` lasciato al valore di default. Gli script di ogni agente sono stati passati via API in qualità di *System Messages* all'interno di una conversazione.

È possibile trovare le porzioni di script (meta-regole, prologo, epilogo, script di ruolo) all'interno della cartella `.data` ed è possibile testare la soluzione avviando il programma e inviando la stringa `sum 1 2 3 42` sul terminale.

Invito il lettore a clonare il repository e seguire l'esecuzione *step-by-step*, partendo dal file `Program.cs` per comprendere meglio la logica dietro l'implementazione proposta.



## Il Risultato

L'implementazione con AI ha **funzionato** dopo qualche tentativo, senza il bisogno di grosse correzioni agli script né di configurare i parametri del modello LLM sottostante - ad esempio settando una `temperature` vicina allo zero. Successivamente ho condotto altri test con valori minori di `temperature` e raffinato gli script per ottenere output più puliti sotto il punto di vista meramente sintattico, verificando ulteriormente la correttezza dell'implementazione.

Di seguito uno screenshot di esempio che mostra l'output prodotto dal sistema per l'input `[1, 2, 3, 42]`.

![Agents Workflow Diagram](workflow-output.png)

A fronte dei risultati ottenuti, emergono le seguenti considerazioni, alcune banali, altre meno:

1. Gli agenti implementati tramite OpenAI danno prova di essere dei formidabili parser e produttori di codice JSON corretto,

2. Gli agenti sono perfettamente in grado di comprendere i concetti di lista (vuota e non) e di ultimo elemento, nonché di individuare l'ultimo elemento all'interno di una lista in formato JSON,

3. Gli agenti sono in grado di fornire in output il risultato di semplici operazioni matematiche come somme di numeri interi di poche cifre, tralasciando in questa sede la possibilità di estendere i modelli forniti da OpenAI con delle funzioni programmate,

4. Gli agenti sono in grado di **selezionare** opportunamente un tipo di risposta in base al contesto fornito, ovvero sono in grado di svolgere semplici azioni di selezione (*if-then-else*),

5. Considerando la primitiva di *forward* messa a disposizione dalla macchina virtuale implementata per questo esperimento, i nostri agenti sono in grado di effettuare computazioni **turing-complete**, posto che il LLM sottostante non effettui errori nel fornire il risultato di calcoli aritmetici, e non manifesti altri errori legati alla comprensione del testo o alla quantità di memoria contestuale limitata. La primitiva di *forward* consente infatti di esprimere chiamate ricorsive fra agenti all'interno di un medesimo contesto di lavoro.

È importante notare come, all'interno degli script di ruolo mostrati in precedenza, i nostri agenti **non** siano stati istruiti sull'insieme di istruzioni messo a disposizione dal runtime. 

Non hanno mai ricevuto una specifica esatta e completa delle primitive utilizzabili. Si limitano semplicemente, per adesso, ad analizzare la forma dell'input e a rispondere con la struttura sintattica che gli sembra più adatta in base alle direttive ricevute, sostituendo opportunamente i *placeholder* che rappresentano parametri - intuendo in modo piuttosto intelligente quali sono i placeholder all'interno dei JSON mostrati nei rispettivi script ed individuando i valori corrispondenti dagli input successivi.

Nel futuro potrebbe essere possibile progettare degli agenti che, data in input una specifica di *instruction set*, utilizzano autonomamente le primitive del runtime per arrivare ai risultati desiderati (v. sezione successiva, punto 5).

Per le chiamate a OpenAI necessarie a completare l'esperimento in tutte le sue parti ho speso, ironicamente, **$0.02**. Posso dire a questo punto di aver dato i miei *two-cents* al settore dell'AI e di aver scoperto il modo più costoso per sommare una lista di numeri.



## Previsioni per il Futuro

Per quanto riguarda l'esperimento, penso che il processo e il materiale prodotto finora siano abbastanza stimolanti da meritare una continuazione in un progetto dedicato, open source o orientato al mercato. Per quanto riguarda il futuro del mondo del lavoro, rimando alla sezione successiva contenente un'[opinione personale](#comment). 

Sintetizzo invece qui le valutazioni che mi sembrano più oggettive e le previsioni più probabili per il futuro dell'ingegneria del software.

1. Come questo banale esperimento dimostra, è già possibile creare agenti AI che collaborano tra loro per svolgere compiti più o meno complessi - in parte algoritmici, in parte intelligenti - condividendo informazione e coordinandosi per risolvere problemi.

2. La complessità dei workflow in cui saranno coinvolte le AI sta già aumentando. Gli agenti non si limiteranno a sommare liste di numeri ma eseguiranno operazioni complesse come leggere, scrivere e scambiarsi documenti, interrogare database aziendali, leggere file su disco, effettuare chiamate API per agire concretamente sul mondo esterno e così via.

3. Ne deriva che, in un futuro non molto distante, le AI saranno in grado non solo di lavorare su attività circostanziate (produrre un testo, tradurre un testo) ma di aggregare e analizzare **informazioni sparse** per navigare in ambienti cross-funzionali complessi e impersonare ruoli altrettanto complessi (redigere un bilancio aziendale a partire dai documenti contabili, creare e postare autonomamente contenuti sui canali social aziendali, pubblicare un job posting su LinkedIn e schedulare automaticamente una videochiamata con il candidato migliore, e così via all'infinito).

4. Se un sistema informatico diventa, banalmente ed essenzialmente, una specfica testuale, realizzando il sogno ultimo dell'analisi dei requisiti di trasformare un pezzo di testo scritto in linguaggio naturale in un sistema funzionante, allora tutti i sistemi informatici saranno **auto-programmabili**. Esisterà un agente AI specializzato nel produrre una specifica di sistema (qualcosa di simile allo script mostrato in questo esperimento) volta a simulare il funzionamento di un'organizzazione aziendale o di un suo ramo. Il futuro potrebbe essere fatto di **agenti che generano organizzazioni di agenti** per risolvere problemi di business.

5. Questa visione, che potrebbe sembrare futuristica, potrebbe essere in realtà già superata, riduttiva. Abbiamo *supposto* infatti che gli agenti abbiano bisogno di una specifica di regole molto dettagliata, generata da un attore intelligente, per poter risolvere un problema concreto. Nulla vieta che un gruppo di agenti, opportunamente istruito con delle meta-regole abbastanza efficaci, possa essere così intelligente da essere in grado di **auto-organizzarsi**. In altre parole, data la formulazione di un problema in input, essere in grado di trovare una soluzione formale al problema, quindi di comunicare e cooperare internamente per auto-assegnarsi ruoli e compiti, senza che siano necessarie altre indicazioni ad esclusione della specifica del problema da risolvere e dell'ambiente di esecuzione - inferendo autonomamente sia la soluzione astratta e che l'implementazione concreta. Il futuro anteriore potrebbe essere fatto di **società auto-determinate di agenti**.

6. Ci avviamo verso un'informatica dominata dall'**AIOps** e da un paradigma tecnico-economico di **Organization-as-a-Service**. Potremo comprare organizzazioni virtuali composte da persone virtuali, generando i nostri professionisti in capacità e competenze a nostro piacimento. Potremo creare comporre le nostre aziende come se fossero mattoncini di un Lego personalizzato, dinamicamente trasformabile, scalabile e supervisionato da esseri umani *in-the-middle*, con l'unico vincolo del costo economico ed energetico. Fondendo le parole *agente* e *organizzazione*, potremmo dire che il futuro prossimo sarà l'era dell'**agentizzazione**.



## <a name="comment"></a>Il Futuro del Lavoro Intellettuale

**Attenzione**. Questa sezione contiene opinioni personali senza alcuna pretesa di oggettività.

I sistemi di AI come ChatGPT sono stati inizialmente concepiti come *chatbot*, cioè agenti conversazionali orientati al dialogo e all'interazione con un essere umano.  Spesso vengono presentati al grande pubblico come dei *copiloti*: assistenti virtuali che aiutano l'utente a svolgere compiti più o meno complessi sotto il proprio comando.

Ancora più spesso le AI vengono esaltate dagli appassionati di tecnologia con il *claim* che esse siano semplicemente degli strumenti. Non una minaccia per il lavoratore, ma solo un *tool* per velocizzare o automatizzare parte del lavoro, liberando gli operatori umani dal fardello di attività noiose e ripetitive.

La narrazione dominante sul tema è chiara e quanto mai positivista. L'AI ci renderà più efficienti e produttivi, chi riuscirà ad inserirla nel proprio *skillset* continuerà ad avere un posto nel mercato del lavoro, chi non riuscirà ad adeguarsi soccomberà (evviva il libero mercato, eccetera). 

Questa visione è in parte vera, ma ritengo che sia enormemente riduttiva e che non riesca a cogliere appieno l'impatto che l'AI avrà sul mondo del lavoro, in particolare nella sfera del lavoro intellettuale e dei servizi, sottostimando grandemente i rischi in favore dei benefici.

Mi stupisco ogni volta quando, leggendo articoli, opinioni e commenti in rete, osservo che i fautori più entusiasti dell'AI sono proprio i lavoratori del settore tecnologico e specialmente i *developer*. Lo considero come un indicatore generale del livello di alfabetizzazione informatica, anche di chi pratica informatica per professione.

L'invenzione dell'AI generativa è l'inizio di una rivoluzione industriale. Con la precedente rivoluzione industriale, quella digitale, l'automazione (stupida) delle macchine ha sostituito gli operai nelle catene di montaggio delle fabbriche. 

La quarta rivoluzione industriale, quella attualmente in essere e già fondata sull'avvento del cloud e dell'internet pervasivo, vedrà l'automazione (questa volta meno stupida) delle macchine sostituire gli operai nelle catene di montaggio dei servizi informativi, cioè nei processi intellettuali di analisi, progettazione e decisione riguardanti tutto ciò che non è materiale. Non interesserà solamente il *saper fare* ma anche il *sapere* e il *conoscere*.

Subiranno l'onda d'urto tutte le figure professionali che esistono solo in ragione della loro attività puramente intellettuale, non-manuale, svincolata dalla realtà fisica. Quindi tutti i lavoratori del digitale, della comunicazione e dell'informazione, dei servizi e della burocrazia, ma anche i professionisti della consulenza e coloro che storicamente sono stati i depositari della conoscenza e della competenza specifica: avvocati, ingegneri, manager, forse persino i medici.

Ho la sensazione che, contrariamente a quanto sostiene l'opinione diffusa sull'argomento, questo cambiamento non creerà più posti di lavoro di quanti ne distruggerà, ma che invece causerà un bel po' di problemi a molte persone.

Il motivo è il seguente: la velocità con la quale l'AI si insinuerà nei processi aziendali ed economici (una manciata di anni) non darà il tempo, alla maggior parte dei lavoratori interessati, di metabolizzare il cambiamento e riconvertirsi in qualche nuova e improbabile figura professionale di cui oltretutto forse non ci sarà più il bisogno.

Questo filo di ragionamento conduce inevitabilmente a riflessioni di carattere politico e sociale che non sono oggetto di questo commento.

Oggetto di questo commento è invece l'invito a **prepararsi**, senza voler esagerare ma neanche minimizzare i rischi, rivolto in particolare a chi ha in mente di iniziare una carriera nel settore dell'informatica per guadagnarsi da vivere - *front-end developer*, *full-stack developer*, *back-end developer*, *data analyst*, eccetera - che dalla rivoluzione AI ne uscirà stravolto. 


## Contatti

Chi desiderasse contattarmi per domande o approfondimenti può scrivermi una mail a (info [at] federicocorrao [dot] it). Sarò felice di rispondere!
