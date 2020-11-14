module MARSApp.Model.OptionModel

open MARSApp.Model.ConfigurationModel
open System

type BlackScholesOptionValuationModel =
    member this.SimulateGBM() =
        //generates list of n Uniform RVs from interval [0,1]; here it's [0,1) I guess
        let genRandomNumbersNominalInterval (count:int) (seed:int) : float list=
            let rnd = System.Random(seed)
            List.init count (fun _ -> rnd.NextDouble())

        //input: UniformRM need to be from interval (0,1]
        //input: steps MUST BE EVEN!
        //output: NormalRV have mean=0 and standard_deviation=1
        let normalizeRec (uniformList:float list) (n:int) : float list =
            let rec buildNormalList (normalList:float list) =
                if normalList.Length = n then normalList
                else
                    let currentNIdOne = normalList.Length
                    let currentNIdTwo = currentNIdOne + 1
                    let oneU = uniformList.[currentNIdOne]
                    let twoU = uniformList.[currentNIdTwo]
                    let oneN = sqrt(-2.*Math.Log(oneU, Math.E))*sin(2.*Math.PI*twoU)
                    let twoN = sqrt(-2.*Math.Log(oneU, Math.E))*cos(2.*Math.PI*twoU)
                    let newUniforms = [oneN; twoN]
                    buildNormalList (normalList@newUniforms)
            buildNormalList []

        let simulateGBM (count:int) (steps:int) (price:float) (drift:float) (vol:float) (years:int) (seed:int) =
            //start counting t(trajectories)
            let rec buildResult currentResult t =
                if t = count+1 then currentResult
                else
                    let normalRV = normalizeRec (genRandomNumbersNominalInterval steps t) steps
        
                    //build stock prices list
                    let rec buildStockPricesList (currentStockPricesList:float list) (steps:int) (normalId:int) : float list =
                        if normalId = steps-1 then currentStockPricesList
                        else
                            let firstExpTerm =  (drift - (vol**2.)/2.) * (float(years)/float(steps))
                            let secondExpTerm =  vol * sqrt(float(years)/float(steps)) * normalRV.[normalId]
                            let newStockPrice = currentStockPricesList.[normalId] * Math.E ** (firstExpTerm + secondExpTerm)
                            buildStockPricesList (currentStockPricesList@[newStockPrice]) steps (normalId+1)
                    let stockPricesList = buildStockPricesList [price] steps 0
                    //printfn "StockPricesList: %A" stockPricesList

                    let finalStockPrice = stockPricesList.[stockPricesList.Length - 1]
                    //calculate historical (realized) volatility
                    let rec buildRList (rList:float list) (index:int) =
                        if index = steps-1 then rList
                        else
                            let currentR =  Math.Log((stockPricesList.[index+1])/(stockPricesList.[index]), Math.E)
                            buildRList (rList@[currentR]) (index+1)

                    let rList = buildRList [] 0
                    let rAvg = List.average rList
                    let sumOfSquares : float = List.fold (fun acc elem -> acc  + (elem - rAvg)**2.) 0. rList
                    let historicalVolatilitySquared = float(steps)/(float(years)*(float(steps)-1.)) * sumOfSquares
                    //prepare final result being tuple: (finalStockPrice, realizedVolatility)
                    let newResult = finalStockPrice
                    buildResult (currentResult@[newResult]) (t+1)
            let result = buildResult [] 1
            result

        let count = 1000
        let steps = 250 //must be EVEN!
        let price = 4.20
        let drift = 0.12
        let vol = 0.2
        let years = 1
        let seed = 5

        simulateGBM count steps price drift vol years seed

