###################################################################
#    Prepare Libraries
###################################################################
{
  # install.packages('Rcpp')
  # install.packages('multicool')
  # install.packages('rugarch')
  # install.packages('coda')
  # install.packages('stochvol')
  # install.packages('shiny')
  # install.packages('tsoutliers')
  # install.packages('pracma')
  # install.packages('VGAM')
  # install.packages('MASS')
  # install.packages('ExtDist')
  # install.packages('survival')
  # install.packages('Hmisc')
  # install.packages('copula')
  # install.packages('distr')
  # {
  #  install.packages('drat')
  #  drat::addRepo('ghrr')
  #  install.packages('RQuantLib', type='binary')
  # }
  # install.packages('bizdays')
  #
  # install.packages('moments')
  # install.packages('snow')
  # install.packages('parallel')
  # install.packages('foreach')
  # install.packages('doParallel')
  # install.packages('xts')
  # install.packages('rmgarch')
  
  
  require('Rcpp')
  require('shiny')
  require('parallel')
  require('multicool')
  require('rugarch')
  require('reshape2')
  require('coda')
  require('stochvol')
  require('tsoutliers')
  require('pracma')
  require('VGAM')
  require('MASS')
  require('ExtDist')
  require('survival')
  require('Hmisc')
  require('copula')
  require('distr')
  require('RQuantLib')
  require('bizdays')
  require('moments')
  require('parallel')
  require('foreach')
  require('doParallel')
  require('xts')
  require('rmgarch')
  require('DEoptim')
  require('plyr')
  
  Parent.WD = 'C:/Users/Samuel/Documents/dfkjdf/'
  
  asset.1 = list(symbol = 'T', Expiry = as.Date('2017-08-04'), Strike = 31,Right = 'PUT')
  asset.2 = list(symbol = 'IBM', Expiry = as.Date('2017-08-18'), Strike = 130,Right = 'PUT')
  asset.3 = list(symbol = 'MCD', Expiry = as.Date('2017-09-15'), Strike = 135,Right = 'PUT')
  # asset.4 = list(symbol = 'MMM', Expiry = as.Date('2017-08-18'), Strike = 185,Right = 'PUT')
  # asset.5 = list(symbol = 'MCD', Expiry = as.Date('2017-08-18'), Strike = 130,Right = 'PUT')
  # asset.6 = list(symbol = 'DIS', Expiry = as.Date('2017-08-18'), Strike = 90,Right = 'PUT')
  # asset.7 = list(symbol = 'MCD', Expiry = as.Date('2017-08-18'), Strike = 135,Right = 'PUT')
  # asset.8 = list(symbol = 'EBAY', Expiry = as.Date('2017-09-15'), Strike = 29,Right = 'PUT')
  # asset.9 = list(symbol = 'ORCL', Expiry = as.Date('2017-09-15'), Strike = 45,Right = 'PUT')
  # asset.10 = list(symbol = 'MCD', Expiry = as.Date('2017-08-25'), Strike = 139,Right = 'PUT')
  # asset.11 = list(symbol = 'ORCL', Expiry = as.Date('2017-09-15'), Strike = 44,Right = 'PUT')
  assets = lapply(1:3,function(x) return (get(paste0('asset.', x))))
  
  SymbolList = unique(sapply(assets,function(x) return(x[['symbol']])))
}



###################################################################
#    Set Parameters
###################################################################
{
  BackwardWaveLen  = 0
  ForwardWaveLen  = 1
  WaveLen = BackwardWaveLen + ForwardWaveLen + 1
  Include.SPX = 0
}


###################################################################
#    Prepare Data Sets
###################################################################
{
  Get.Clean.DatePrice = function(wd) {
    ####### Read in data  #############
    setwd(wd)
    Data_Prices = read.csv('Prices.csv', T)
    Data_Dates = read.csv('Dates.csv', T)
    Prices_Original = Data_Prices$Close
    N = length(Prices_Original)
    EventType = as.vector(c('Earnings', 'FOMC', 'Xtreme'), mode = 'list')
    
    
    ####### Define Dates and Event Dates #############
    Date_Original = as.Date(as.character(Data_Prices$Date), '%Y-%m-%d')
    Prices_Original = Prices_Original[order(Date_Original)]
    Date_Original = sort(Date_Original)
    
    List_EventDates = lapply(EventType, function(x) {
      as.Date(as.character(Data_Dates$Date[Data_Dates$Type == x]), '%m/%d/%Y')
    })
    names(List_EventDates) = EventType
    load_quantlib_calendars('UnitedStates/NYSE',
                            from = min(Date_Original) - 50,
                            to = '2021-12-29')
    
    
    ### Trim Date to make sure whole wave exists
    WavelengthExistVector = function(stage, EventName)
    {
      return(sapply(List_EventDates[[EventName]], function(x)
        return(
          offset(x, stage, 'QuantLib/UnitedStates/NYSE') %in% Date_Original[-1]
        )))
    }
    List_ExistVector = lapply(EventType, function(x) {
      1 == apply(cbind(sapply(
        seq(-BackwardWaveLen, ForwardWaveLen),
        WavelengthExistVector,
        x
      )), 1, prod)
    })
    names(List_ExistVector) = EventType
    List_EventDates = lapply(EventType, function(x)
      List_EventDates[[x]] = List_EventDates[[x]][List_ExistVector[[x]] == T])
    names(List_EventDates) = EventType
    
    
    ####### Define Event Indicator #############
    CreateIndicator = function(WaveSize, GrandDates, TargetDates)
    {
      ResultIndicator = rep(F, length(GrandDates))
      ResultIndicator[pmax(match(TargetDates, GrandDates) + WaveSize, 1)] = T
      return(ResultIndicator)
    }
    
    List_EventDatesLogic = lapply(EventType, function(x) {
      apply(cbind(
        sapply(
          seq(-BackwardWaveLen, ForwardWaveLen),
          CreateIndicator,
          Date_Original,
          List_EventDates[[x]]
        )
      )
      , 1, function(y) {
        return (T %in% y)
      })
    })
    names(List_EventDatesLogic) = EventType

    AllEventsLogic = apply(matrix(unlist(List_EventDatesLogic), ncol = length(EventType)), 1,
                           function(x) {
                             return(T %in% x)
                           })
    
    
    ####### Define Event Stage Identifier #############
    List_EventIdentifier = lapply(EventType, function(x) {
      return(cbind(
        sapply(
          seq(-BackwardWaveLen, ForwardWaveLen),
          CreateIndicator,
          Date_Original,
          List_EventDates[[x]]
        )
      ) %*% (1:(WaveLen)))
    })
    names(List_EventIdentifier) = EventType
    
    
    
    ##### 1. Return_Original ######
    Return_Original = log(Prices_Original[-1] / Prices_Original[-N])
    
    
    
    ##### 2. Return_ExEvent ######
    Return_ExEvent = Return_Original
    Mean_ExEvent = mean(Return_ExEvent[AllEventsLogic[-1] == F])
    Sd_ExEvent = sd(Return_ExEvent[AllEventsLogic[-1] == F])
    Return_ExEvent[AllEventsLogic[-1] == T] = rnorm(length(which(AllEventsLogic[-1] ==
                                                                   T)), Mean_ExEvent, 0) #Sd_ExEvent)
    
    return(list(Date = Date_Original[-1], Return = as.ts(Return_ExEvent)))
  }
  DatePrices = lapply(paste0(Parent.WD, SymbolList), Get.Clean.DatePrice)
  names(DatePrices) = SymbolList
  
  
  ########### Consolidate Date ###########
  Date.Raw = unique(as.numeric(unlist(sapply(SymbolList, function(x) {
    return(DatePrices[[x]]$Date)
  }))))
  Date.Final = as.Date(Date.Raw[order(Date.Raw)])
  
  
  ########### Consolidate Prices ###########
  Return.Final = lapply(lapply(SymbolList, function(x) {
    return(DatePrices[[x]]$Return[match(Date.Final, DatePrices[[x]]$Date)])
  }), na.locf)
  Final.DF = data.frame(matrix(
    unlist(Return.Final),
    byrow = F,
    ncol = length(SymbolList)
  ))
  names(Final.DF) = SymbolList
  Final.DF$Date = Date.Final
}



###################################################################
#    Fit Multivate-GARCH Model to ExEvent Returns
###################################################################
{
  noise = c('norm',
            'snorm',
            'std',
            'sstd',
            'ged',
            'sged',
            'nig',
            'ghyp',
            'jsu')
  ARMA.Order = c(2, 2)
  
  GetMaxLLSpec = function(symbol) {
    Ret_ExEvent = as.ts(Final.DF[symbol])
    # Remove all additive outliers
    tsoModel = tso(
      y = Ret_ExEvent,
      types = c('AO'),
      tsmethod = 'auto.arima',
      args.tsmodel = list(model = 'local-level')
    )
    
    # Fit Garch with noise type of Max LL
    GarchLL = function(noiseType, inputData)
    {
      spec = ugarchspec(
        mean.model = list(armaOrder = ARMA.Order, include.mean = TRUE),
        variance.model = list(model = 'gjrGARCH'),
        distribution.model = noiseType
      )
      fit = ugarchfit(spec, data = inputData, solver = 'hybrid')
      return(likelihood(fit))
    }
    GarchLL  = sapply(noise, GarchLL, tsoModel$yadj)
    maxGarchLLIndex = which(GarchLL == max(GarchLL))
    maxGarchSpec = ugarchspec(
      mean.model = list(armaOrder = ARMA.Order, include.mean = TRUE),
      variance.model = list(model = 'gjrGARCH'),
      distribution.model = noise[maxGarchLLIndex]
    )
    maxGarchFit = ugarchfit(maxGarchSpec, data = tsoModel$yadj, solver = 'hybrid')
    return(list(Fit = maxGarchFit, Spec = maxGarchSpec))
    # return( list(Spec =maxGarchSpec ))
  }
  
  tso.Remove = function(symbol) {
    Ret_ExEvent = as.ts(Final.DF[symbol])
    tsoModel = tso(
      y = Ret_ExEvent,
      types = c('AO'),
      tsmethod = 'auto.arima',
      args.tsmodel = list(model = 'local-level')
    )
    return(tsoModel$yadj)
  }
  
  Final.DF.oF = data.frame(matrix(
    unlist(lapply(SymbolList, tso.Remove)),
    byrow = F,
    ncol = length(SymbolList)
  ))
  names(Final.DF.oF) = SymbolList
  
  
  ###### Attach SPX to dataframe ######
  if(Include.SPX==1)
  {
    setwd(paste0(Parent.WD, 'SPX'))
    SPX.Raw = read.csv('Prices.csv')
    SPX.DF = data.frame(Date = as.Date(SPX.Raw$Date,'%m/%d/%Y'),Price = as.numeric(SPX.Raw$Close))
    SPX.DF = SPX.DF[order(SPX.DF$Date),]
    SPX.DF$Return = c(NA,log(SPX.DF$Price[-1])-log(SPX.DF$Price[-length(SPX.DF$Price)]))
    Final.DF.oF$SPX = na.locf(SPX.DF$Return[match(Date.Final,SPX.DF$Date)])  
  }
  
  
  
  ###### Attach date to dataframe ######
  Final.DF.oF$Date = Date.Final
  
  
  ###### Parallel Garch Spec Generation ######
  no_cores <<- detectCores() - 1
  cl <<- makeCluster(no_cores, outfile = '')
  clusterExport(cl, ls(.GlobalEnv))
  dummy = clusterEvalQ(cl, library(rugarch))
  dummy = clusterEvalQ(cl, library(tsoutliers))
  SpecAndFit = parLapply(cl, SymbolList, GetMaxLLSpec)
  stopCluster(cl)
  FitList = lapply(SpecAndFit, function(x)
    return(x$Fit))
  SpecList = lapply(SpecAndFit, function(x)
    return(x$Spec))
  # Specs.GARCHs = multispec(SpecList)
  
  
  
  ###### Train the garch SPX model ######
  if(Include.SPX==1){
    SPX.GarchOrder = c(2,2)
    
    GarchLL = function(noiseType, inputData)
    {
      spec = ugarchspec(
        mean.model = list(armaOrder = ARMA.Order, include.mean = TRUE),
        variance.model = list(model = 'gjrGARCH',garchOrder = SPX.GarchOrder),
        distribution.model = noiseType
      )
      fit = ugarchfit(spec, data = inputData, solver = 'hybrid')
      return(likelihood(fit))
    }
    
    GarchLLs  = sapply(noise, GarchLL, Final.DF.oF$SPX)
    
    maxGarchSpec.SPX = ugarchspec(
      mean.model = list(armaOrder = ARMA.Order, include.mean = TRUE),
      variance.model = list(model = 'gjrGARCH',garchOrder = SPX.GarchOrder),
      distribution.model = noise[which(GarchLLs == max(GarchLLs))]
    )
    
    SpecList[[length(SpecList)+1]] = maxGarchSpec.SPX
  }

  
  Specs.GARCHs = multispec(SpecList)
  
  ###### Fit the mgarch model ######
  Specs.DCC = dccspec(
    Specs.GARCHs,
    VAR = F,
    dccOrder = c(1, 1),
    model = 'DCC',
    distribution = 'mvlaplace'
  )
  Final.xts = as.xts(Final.DF.oF[, -(1 + length(SymbolList)+Include.SPX)], order.by =
                       Final.DF.oF[, length(SymbolList)+Include.SPX + 1])
  cl <<- makeCluster(no_cores, outfile = '')
  clusterExport(cl, ls(.GlobalEnv))
  DCC.Model = dccfit(
    spec =  Specs.DCC,
    data = Final.xts,
    solver = c('hybrid', 'solnp'),
    cluster = cl
  )
  stopCluster(cl)
}


###################################################################
#    Analyze correlation
###################################################################
if(Include.SPX==1)
{
  
  corrSeries = rcor(DCC.Model)
  Mean.Corr.Series = aaply(corrSeries,3,function(x){
    # return(mean(x[lower.tri(x)]))
    return(mean(x[nrow(x),-nrow(x)]))
  })
  
  ########## Visualize plot ##########
  plot(x = Final.DF.oF$Date, y = Mean.Corr.Series,xaxt = "n", type = 'l')
  DateLabel = Final.DF.oF$Date[seq(from = 1, to = length(Final.DF.oF$Date),by = 224/2)]
  axis(side = 1,at = DateLabel, labels = DateLabel,cex = .3)
  grid (NULL,NULL, lty = 6, col = "grey60")
  
  Pts.Interested.x = as.Date(c('2015-06-12','2016-06-24'),'%Y-%m-%d')
  Pts.Interested.y = Mean.Corr.Series[Final.DF.oF$Date %in% Pts.Interested.x]
  abline(v = Pts.Interested.x, col = 'red')
  
  par(new = T)
  plot(x = Final.DF.oF$Date, y = Final.DF.oF$SPX, type = 'l',col = 'blue',lwd = 1.5,axes = F)
  axis(4)
  
  par(new = T)
  plot(x = Final.DF.oF$Date, y = SPX.DF$Price[-1], type = 'l',col = 'blue',lwd = 1.5,axes = F)
  axis(4)
}



###################################################################
#    Generate New Time Series
###################################################################
{
  ####### Simulation Initialization #############
  {
    NPaths = 10000
    SimulationEndDate = preceding(as.Date("2018-12-31", "%Y-%m-%d"),
                                  'QuantLib/UnitedStates/NYSE')
    SimulationStartDate = adjust("UnitedStates/NYSE", max(Final.DF.oF$Date, 1) +
                                   1)
    NumOfDays = bizdays(SimulationStartDate,
                        SimulationEndDate,
                        'QuantLib/UnitedStates/NYSE') + 1
    Date_Future = bizseq(SimulationStartDate,SimulationEndDate,'QuantLib/UnitedStates/NYSE')
  }
  
  ####### Read in Shock Matricess  #############
  {
    Read.ShockMatrix = function(futureDates, FilePath){
      DataIn = read.csv(FilePath,header = F)
      Date.Index = match(futureDates,as.Date(DataIn[,1],'1970-01-01'))
      return(as.matrix(DataIn[,-1])[Date.Index,])
    }
    List.ShockMatrices = lapply(paste0(Parent.WD, SymbolList, '/ShockMatrix.csv'), function(fileAdd) {
      return(Read.ShockMatrix(Date_Future, fileAdd))
    })
    names(List.ShockMatrices) = SymbolList
  }
  
  
  
  ####### Simulate m-GARCH process  #############
  mGARCHsim = dccsim(
    DCC.Model,
    n.sim = NumOfDays,
    m.sim = NPaths,
    startMethod =  'sample' ,
    rseed = 0
  )
  Simmed.Path = mGARCHsim@msim$simX
  Simmed.Path.rbind = do.call("rbind", Simmed.Path)
  Simmed.Path.List = lapply((1:(length(SymbolList)+Include.SPX)), function(i) {
    return(matrix(Simmed.Path.rbind[, i], byrow = F, ncol = NPaths))
  })
  names(Simmed.Path.List) = SymbolList
  if(Include.SPX == 1){
    names(Simmed.Path.List)[length(SymbolList)+1] = 'SPX'
  }
  
  
  
  ############### combined process with Shocks #############
  for(i in (1:length(SymbolList))){
    symbol = SymbolList[i]
    Simmed.Path.List[[symbol]][List.ShockMatrices[[symbol]][,1]!=0,] = 0
    Simmed.Path.List[[symbol]] = Simmed.Path.List[[symbol]] + List.ShockMatrices[[symbol]]  
  }
  rm(mGARCHsim)
  rm(Simmed.Path)
  rm(List.ShockMatrices)
  gc()
}


###################################################################
#       Usefull function for calculation
###################################################################
{
  Calculate_Fitted.Expiry.Prices = function(symbol,ExpiryDate)
  {
    ####### Read in data  #############
    setwd(paste0(Parent.WD,'/',symbol ))
    Data_Prices = read.csv('Prices.csv', T)
    LastClose = Data_Prices$Close[as.Date(Data_Prices$Date,'%Y-%m-%d') == max(Final.DF$Date)]
    
    MaturityLength = bizdays(SimulationStartDate, ExpiryDate, 'QuantLib/UnitedStates/NYSE')+1
    
    Fitted.Expiry.Returns = apply(Simmed.Path.List[[symbol]],2,function(x){
      prod(exp(head(x,MaturityLength)))
    })
    return(LastClose*Fitted.Expiry.Returns)
  }
  
  payoff = function(ST,K,Premium,Right = 'PUT'){
    direction = 2*(Right == 'PUT')-1
    return(Premium - max(direction*(K-ST),0))
  }
  
  Probability.OTM = function(K,Sim.Prices, Right = 'PUT'){
    direction = 2*(Right == 'PUT')-1
    return(sum(direction*(Sim.Prices-K) > 0)/NPaths)
  }
  
  Cond.Loss = function(K,Sim.Prices, Right = 'PUT'){
    direction = 2*(Right == 'PUT')-1
    conLoss = mean(direction*(K-Sim.Prices[(direction*(K-Sim.Prices))>0]))
    if(is.na(conLoss)) conLoss = 0
    return(conLoss)
  }
  
  ExReturn = function(K,Sim.Prices,Premium,Right = 'PUT'){
    return(mean(sapply(Sim.Prices,payoff,K,Premium,Right)))
  }
  
  SDReturn = function(K,Sim.Prices,Premium,Right = 'PUT'){
    return(sd(sapply(Sim.Prices,payoff,K,Premium,Right)))
  }
}



###################################################################
#       Calculate correlation between assets
###################################################################
{

  GetCov = function(asset.A, asset.B) {
    assetss = list(asset.A, asset.B)
    Sim.Prices = lapply(1:2, function(x) {
      return(Calculate_Fitted.Expiry.Prices(assetss[[x]]$symbol, assetss[[x]]$Expiry))
    })
    Returns = lapply(1:2, function(x) {
      return(
        sapply(
          Sim.Prices[[x]],
          payoff,
          assetss[[x]]$Strike,
          assetss[[x]]$Premium,
          assetss[[x]]$Right
        ) / assetss[[x]]$Margin * 780
      )
    })
    
    return(max(2^-20,cov(Returns[[1]],Returns[[2]])))
  }
  
  
  Cov.Matrix = do.call('rbind',lapply(assets,function(x) return(sapply(assets,GetCov,x))))
}



###################################################################
#       Evaluate optimal portfolio
###################################################################
{
    ExcessMargin = 400000
    
    Sharpe = function(N,assets,CovMatrix){
      Returns = sapply(assets,function(x) x[['ER']])
      Mar.Matrix = diag(sapply(assets,function(x) x[['Margin']]))
      return(N%*%Mar.Matrix%*%Returns/sqrt(N%*%Mar.Matrix%*%CovMatrix%*%Mar.Matrix%*%N))
    }
    Return.Function = function(N,assets){
      Returns = sapply(assets,function(x) x[['ER']])
      Mar.Matrix = diag(sapply(assets,function(x) x[['Margin']]))
      return(N%*%Mar.Matrix%*%Returns)
    }
    SD.Function = function(N,assets,CovMatrix){
      Mar.Matrix = diag(sapply(assets,function(x) x[['Margin']]))
      return(sqrt(N%*%Mar.Matrix%*%CovMatrix%*%Mar.Matrix%*%N))
    }
    ReqMargin = function(N,assets){
      return(N%*%(sapply(assets,function(x) x[['Margin']])))
    }
    Split.n.Get = function(x,n,i){
      return(split(x, ceiling(seq_along(x)/n))[[j]])
    }
    Sharpe.Modified = function(N){
      ##### Set up restrictions #####
      
      # If exceed margin
      if(ReqMargin(N,assets)>ExcessMargin){
        return (0)
      }
      
      # If any negative
      if(T%in%(N<0)){
        return(0)
      }
      
      # If any negative
      if(sum(N)==0){
        return(0)
      }
      
      ##### Return functions #####
      return(-Sharpe(N,assets,Cov.Matrix))
    }
    Return.Modified = function(N){
      ##### Set up restrictions #####
      
      # If exceed margin
      if(ReqMargin(N,assets)>ExcessMargin){
        return (0)
      }
      
      # If any negative
      if(T%in%(N<0)){
        return(0)
      }
      
      # If any negative
      if(sum(N)==0){
        return(0)
      }
      
      ##### Return functions #####
      return(-Return.Function(N,assets))
    }
    
    Max.Holding = sapply(1:length(assets),function(x) return(ExcessMargin%/%assets[[x]]$Margin))
    
    bb = rep(1,length(assets))
    for (i in 1:10){
      Model.MaxSharpe = DEoptim(fn=Sharpe.Modified,lower=rep(0,length(assets)),upper = rep(1,length(assets)),
                                control = DEoptim.control(NP = 1000,itermax = 4000, trace = T,p = .1),fnMap = round)
      Return.Function(Model.MaxSharpe$optim$bestmem,assets)*(ExcessMargin%/%ReqMargin(Model.MaxSharpe$optim$bestmem,assets))
      Model.MaxSharpe$optim$bestval  
      bb = Model.MaxReturn$optim$bestmem
      bb = bb + 1#(bb>0)*1
    }
    
    
    bb = rep(5,length(assets))
    for(i in 21:30){
      Model.MaxReturn = DEoptim(fn=Return.Modified,lower=rep(0,length(assets)),upper = bb,
                                control = DEoptim.control(NP = 1000,itermax = 1000, trace = T,p = .1),fnMap = round)
      Model.MaxReturn$optim$bestval
      Sharpe(Model.MaxReturn$optim$bestmem,assets,Cov.Matrix)
      bb = Model.MaxReturn$optim$bestmem
      bb = bb + (bb>0)*1
    }
 
}