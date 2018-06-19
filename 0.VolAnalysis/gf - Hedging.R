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
  
  asset.1 = list(symbol = 'TDG', Expiry = as.Date('2017-09-15'), Strike = 220,Right = 'PUT', Premium = 1.446532, Position = -1)
  asset.2 = list(symbol = 'EL', Expiry = as.Date('2017-10-20'), Strike = 80,Right = 'PUT', Premium = 0.19656, Position = -1)
  asset.3 = list(symbol = 'PM', Expiry = as.Date('2017-09-15'), Strike = 90,Right = 'PUT', Premium = 0.09206269, Position = -1)
  asset.4 = list(symbol = 'MMM', Expiry = as.Date('2017-09-15'), Strike = 185,Right = 'PUT', Premium = 0.479038, Position = -1)
  asset.5 = list(symbol = 'PG', Expiry = as.Date('2017-10-20'), Strike = 75,Right = 'PUT', Premium = 0.192044, Position = -1)
  asset.6 = list(symbol = 'TIF', Expiry = as.Date('2017-08-25'), Strike = 75,Right = 'PUT', Premium = 0.152061304, Position = -1)
  asset.7 = list(symbol = 'V', Expiry = as.Date('2017-09-15'), Strike = 87.5,Right = 'PUT', Premium = 0.134261535, Position = -1)
  asset.8 = list(symbol = 'CRM', Expiry = as.Date('2017-08-25'), Strike = 75,Right = 'PUT', Premium = 0.369056, Position = -1)
  asset.9 = list(symbol = 'ORCL', Expiry = as.Date('2017-09-08'), Strike = 44,Right = 'PUT', Premium = 0.0920626665, Position = -3)
  asset.10 = list(symbol = 'PM', Expiry = as.Date('2017-09-15'), Strike = 87.5,Right = 'PUT', Premium = 0.09206269, Position = -1)
  asset.11 = list(symbol = 'MCD', Expiry = as.Date('2017-09-01'), Strike = 138,Right = 'PUT', Premium = 0.482038, Position = -1)
  asset.12 = list(symbol = 'TIF', Expiry = as.Date('2017-08-18'), Strike = 77.5,Right = 'PUT', Premium = 0.132046, Position = -1)
  asset.13 = list(symbol = 'AMZN', Expiry = as.Date('2017-10-20'), Strike = 785,Right = 'PUT', Premium = 1.792023, Position = -1)
  asset.14 = list(symbol = 'AMZN', Expiry = as.Date('2017-10-20'), Strike = 790,Right = 'PUT', Premium = 1.992019, Position = -1)
  asset.15 = list(symbol = 'ADSK', Expiry = as.Date('2017-09-15'), Strike = 75,Right = 'PUT', Premium = 0.20206, Position = -1)
  asset.16 = list(symbol = 'MCD', Expiry = as.Date('2017-08-25'), Strike = 138,Right = 'PUT', Premium = 0.3070415, Position = -2)
  asset.17 = list(symbol = 'TIF', Expiry = as.Date('2017-09-15'), Strike = 75,Right = 'PUT', Premium = 0.18906, Position = -1)
  asset.18 = list(symbol = 'MCD', Expiry = as.Date('2017-10-20'), Strike = 130,Right = 'PUT', Premium = 0.302058, Position = -1)
  asset.19 = list(symbol = 'ADBE', Expiry = as.Date('2017-10-20'), Strike = 110,Right = 'PUT', Premium = 0.297058, Position = -2)
  asset.20 = list(symbol = 'ADSK', Expiry = as.Date('2017-09-15'), Strike = 80,Right = 'PUT', Premium = 0.312058, Position = -1)
  asset.21 = list(symbol = 'MMM', Expiry = as.Date('2017-08-25'), Strike = 185,Right = 'PUT', Premium = 0.142061535, Position = -1)
  asset.22 = list(symbol = 'ADSK', Expiry = as.Date('2017-09-15'), Strike = 85,Right = 'PUT', Premium = 0.522053, Position = -1)
  asset.23 = list(symbol = 'CAT', Expiry = as.Date('2017-09-15'), Strike = 90,Right = 'PUT', Premium = 0.115261997, Position = -1)
  asset.24 = list(symbol = 'CAT', Expiry = as.Date('2017-09-08'), Strike = 95,Right = 'PUT', Premium = 0.152061304, Position = -1)
  asset.25 = list(symbol = 'ORCL', Expiry = as.Date('2017-10-20'), Strike = 40,Right = 'PUT', Premium = 0.112062, Position = -1)
  asset.26 = list(symbol = 'PM', Expiry = as.Date('2017-09-08'), Strike = 95,Right = 'PUT', Premium = 0.262059, Position = -1)
  asset.27 = list(symbol = 'BA', Expiry = as.Date('2017-09-01'), Strike = 190,Right = 'PUT', Premium = 0.133261766, Position = -1)
  asset.28 = list(symbol = 'ADSK', Expiry = as.Date('2017-10-20'), Strike = 65,Right = 'PUT', Premium = 0.1573946665, Position = -3)
  asset.29 = list(symbol = 'V', Expiry = as.Date('2017-09-15'), Strike = 92.5,Right = 'PUT', Premium = 0.229559, Position = -1)
  asset.30 = list(symbol = 'ADSK', Expiry = as.Date('2017-10-20'), Strike = 70,Right = 'PUT', Premium = 0.272043, Position = -1)
  asset.31 = list(symbol = 'ADBE', Expiry = as.Date('2017-09-22'), Strike = 115,Right = 'PUT', Premium = 0.252059, Position = -1)
  asset.32 = list(symbol = 'V', Expiry = as.Date('2017-09-08'), Strike = 90,Right = 'PUT', Premium = 0.18906038, Position = -1)
  assets = lapply(1:32,function(x) return (get(paste0('asset.', x))))
  
  SymbolList = unique(sapply(assets,function(x) return(x[['symbol']])))
  
  SPX.Hedges.1= list(Expiry = as.Date('2017-10-19'), Strike = 1000,Premium = 0.075, Right = 'PUT')
  SPX.Hedges.2= list(Expiry = as.Date('2017-10-19'), Strike = 1100,Premium = 0.14, Right = 'PUT')
  SPX.Hedges.3= list(Expiry = as.Date('2017-10-19'), Strike = 1200,Premium = 0.17, Right = 'PUT')
  SPX.Hedges.4= list(Expiry = as.Date('2017-10-19'), Strike = 1225,Premium = 0.185, Right = 'PUT')
  SPX.Hedges.5= list(Expiry = as.Date('2017-10-19'), Strike = 1250,Premium = 0.185, Right = 'PUT')
  SPX.Hedges.6= list(Expiry = as.Date('2017-10-19'), Strike = 1275,Premium = 0.235, Right = 'PUT')
  SPX.Hedges.7= list(Expiry = as.Date('2017-10-19'), Strike = 1300,Premium = 0.285, Right = 'PUT')
  SPX.Hedges.8= list(Expiry = as.Date('2017-10-19'), Strike = 1325,Premium = 0.335, Right = 'PUT')
  SPX.Hedges.9= list(Expiry = as.Date('2017-10-19'), Strike = 1350,Premium = 0.385, Right = 'PUT')
  SPX.Hedges.10= list(Expiry = as.Date('2017-10-19'), Strike = 1375,Premium = 0.435, Right = 'PUT')
  SPX.Hedges.11= list(Expiry = as.Date('2017-10-19'), Strike = 1400,Premium = 0.485, Right = 'PUT')
  SPX.Hedges.12= list(Expiry = as.Date('2017-10-19'), Strike = 1425,Premium = 0.57, Right = 'PUT')
  SPX.Hedges.13= list(Expiry = as.Date('2017-10-19'), Strike = 1450,Premium = 0.635, Right = 'PUT')
  SPX.Hedges.14= list(Expiry = as.Date('2017-10-19'), Strike = 1475,Premium = 0.735, Right = 'PUT')
  SPX.Hedges.15= list(Expiry = as.Date('2017-10-19'), Strike = 1500,Premium = 0.785, Right = 'PUT')
  SPX.Hedges.16= list(Expiry = as.Date('2017-10-19'), Strike = 1525,Premium = 0.885, Right = 'PUT')
  SPX.Hedges.17= list(Expiry = as.Date('2017-10-19'), Strike = 1550,Premium = 0.935, Right = 'PUT')
  SPX.Hedges.18= list(Expiry = as.Date('2017-10-19'), Strike = 1575,Premium = 1.035, Right = 'PUT')
  SPX.Hedges.19= list(Expiry = as.Date('2017-10-19'), Strike = 1600,Premium = 1.135, Right = 'PUT')
  SPX.Hedges.20= list(Expiry = as.Date('2017-10-19'), Strike = 1625,Premium = 1.215, Right = 'PUT')
  SPX.Hedges.21= list(Expiry = as.Date('2017-10-19'), Strike = 1650,Premium = 1.35, Right = 'PUT')
  SPX.Hedges.22= list(Expiry = as.Date('2017-10-19'), Strike = 1675,Premium = 1.465, Right = 'PUT')
  SPX.Hedges.23= list(Expiry = as.Date('2017-10-19'), Strike = 1700,Premium = 1.615, Right = 'PUT')
  SPX.Hedges.24= list(Expiry = as.Date('2017-10-19'), Strike = 1710,Premium = 1.665, Right = 'PUT')
  SPX.Hedges.25= list(Expiry = as.Date('2017-10-19'), Strike = 1720,Premium = 1.73, Right = 'PUT')
  SPX.Hedges.26= list(Expiry = as.Date('2017-10-19'), Strike = 1725,Premium = 1.78, Right = 'PUT')
  SPX.Hedges.27= list(Expiry = as.Date('2017-10-19'), Strike = 1730,Premium = 1.78, Right = 'PUT')
  SPX.Hedges.28= list(Expiry = as.Date('2017-10-19'), Strike = 1740,Premium = 1.88, Right = 'PUT')
  SPX.Hedges.29= list(Expiry = as.Date('2017-10-19'), Strike = 1750,Premium = 1.93, Right = 'PUT')
  SPX.Hedges.30= list(Expiry = as.Date('2017-10-19'), Strike = 1760,Premium = 2.03, Right = 'PUT')
  SPX.Hedges.31= list(Expiry = as.Date('2017-10-19'), Strike = 1770,Premium = 2.095, Right = 'PUT')
  SPX.Hedges.32= list(Expiry = as.Date('2017-10-19'), Strike = 1775,Premium = 2.11, Right = 'PUT')
  SPX.Hedges.33= list(Expiry = as.Date('2017-10-19'), Strike = 1780,Premium = 2.265, Right = 'PUT')
  SPX.Hedges.34= list(Expiry = as.Date('2017-10-19'), Strike = 1790,Premium = 2.28, Right = 'PUT')
  SPX.Hedges.35= list(Expiry = as.Date('2017-10-19'), Strike = 1800,Premium = 2.38, Right = 'PUT')
  SPX.Hedges.36= list(Expiry = as.Date('2017-10-19'), Strike = 1810,Premium = 2.495, Right = 'PUT')
  SPX.Hedges.37= list(Expiry = as.Date('2017-10-19'), Strike = 1820,Premium = 2.58, Right = 'PUT')
  SPX.Hedges.38= list(Expiry = as.Date('2017-10-19'), Strike = 1825,Premium = 2.645, Right = 'PUT')
  SPX.Hedges.39= list(Expiry = as.Date('2017-10-19'), Strike = 1830,Premium = 2.715, Right = 'PUT')
  SPX.Hedges.40= list(Expiry = as.Date('2017-10-19'), Strike = 1840,Premium = 2.78, Right = 'PUT')
  SPX.Hedges.41= list(Expiry = as.Date('2017-10-19'), Strike = 1850,Premium = 2.945, Right = 'PUT')
  SPX.Hedges.42= list(Expiry = as.Date('2017-10-19'), Strike = 1860,Premium = 3.01, Right = 'PUT')
  SPX.Hedges.43= list(Expiry = as.Date('2017-10-19'), Strike = 1870,Premium = 3.075, Right = 'PUT')
  SPX.Hedges.44= list(Expiry = as.Date('2017-10-19'), Strike = 1875,Premium = 3.145, Right = 'PUT')
  SPX.Hedges.45= list(Expiry = as.Date('2017-10-19'), Strike = 1880,Premium = 3.175, Right = 'PUT')
  SPX.Hedges.46= list(Expiry = as.Date('2017-10-19'), Strike = 1890,Premium = 3.31, Right = 'PUT')
  SPX.Hedges.47= list(Expiry = as.Date('2017-10-19'), Strike = 1900,Premium = 3.48, Right = 'PUT')
  SPX.Hedges.48= list(Expiry = as.Date('2017-10-19'), Strike = 1910,Premium = 3.61, Right = 'PUT')
  SPX.Hedges.49= list(Expiry = as.Date('2017-10-19'), Strike = 1920,Premium = 3.71, Right = 'PUT')
  SPX.Hedges.50= list(Expiry = as.Date('2017-10-19'), Strike = 1925,Premium = 3.81, Right = 'PUT')
  SPX.Hedges.51= list(Expiry = as.Date('2017-10-19'), Strike = 1930,Premium = 3.91, Right = 'PUT')
  SPX.Hedges.52= list(Expiry = as.Date('2017-10-19'), Strike = 1940,Premium = 4.04, Right = 'PUT')
  SPX.Hedges.53= list(Expiry = as.Date('2017-10-19'), Strike = 1950,Premium = 4.14, Right = 'PUT')
  SPX.Hedges.54= list(Expiry = as.Date('2017-10-19'), Strike = 1960,Premium = 4.41, Right = 'PUT')
  SPX.Hedges.55= list(Expiry = as.Date('2017-10-19'), Strike = 1970,Premium = 4.54, Right = 'PUT')
  SPX.Hedges.56= list(Expiry = as.Date('2017-10-19'), Strike = 1975,Premium = 4.64, Right = 'PUT')
  SPX.Hedges.57= list(Expiry = as.Date('2017-10-19'), Strike = 1980,Premium = 4.74, Right = 'PUT')
  SPX.Hedges.58= list(Expiry = as.Date('2017-10-19'), Strike = 1985,Premium = 4.84, Right = 'PUT')
  SPX.Hedges.59= list(Expiry = as.Date('2017-10-19'), Strike = 1990,Premium = 4.94, Right = 'PUT')
  SPX.Hedges.60= list(Expiry = as.Date('2017-10-19'), Strike = 1995,Premium = 5.04, Right = 'PUT')
  SPX.Hedges.61= list(Expiry = as.Date('2017-10-19'), Strike = 2000,Premium = 5.17, Right = 'PUT')
  SPX.Hedges.62= list(Expiry = as.Date('2017-10-19'), Strike = 2005,Premium = 5.27, Right = 'PUT')
  SPX.Hedges.63= list(Expiry = as.Date('2017-10-19'), Strike = 2010,Premium = 5.37, Right = 'PUT')
  SPX.Hedges.64= list(Expiry = as.Date('2017-10-19'), Strike = 2015,Premium = 5.54, Right = 'PUT')
  SPX.Hedges.65= list(Expiry = as.Date('2017-10-19'), Strike = 2020,Premium = 5.64, Right = 'PUT')
  SPX.Hedges.66= list(Expiry = as.Date('2017-10-19'), Strike = 2025,Premium = 5.74, Right = 'PUT')
  SPX.Hedges.67= list(Expiry = as.Date('2017-10-19'), Strike = 2030,Premium = 5.87, Right = 'PUT')
  SPX.Hedges.68= list(Expiry = as.Date('2017-10-19'), Strike = 2035,Premium = 6.04, Right = 'PUT')
  SPX.Hedges.69= list(Expiry = as.Date('2017-10-19'), Strike = 2040,Premium = 6.17, Right = 'PUT')
  SPX.Hedges.70= list(Expiry = as.Date('2017-10-19'), Strike = 2045,Premium = 6.34, Right = 'PUT')
  SPX.Hedges.71= list(Expiry = as.Date('2017-10-19'), Strike = 2050,Premium = 6.47, Right = 'PUT')
  SPX.Hedges.72= list(Expiry = as.Date('2017-10-19'), Strike = 2055,Premium = 6.64, Right = 'PUT')
  SPX.Hedges.73= list(Expiry = as.Date('2017-10-19'), Strike = 2060,Premium = 6.77, Right = 'PUT')
  SPX.Hedges.74= list(Expiry = as.Date('2017-10-19'), Strike = 2065,Premium = 6.94, Right = 'PUT')
  SPX.Hedges.75= list(Expiry = as.Date('2017-10-19'), Strike = 2070,Premium = 7.07, Right = 'PUT')
  SPX.Hedges.76= list(Expiry = as.Date('2017-10-19'), Strike = 2075,Premium = 7.24, Right = 'PUT')
  SPX.Hedges.77= list(Expiry = as.Date('2017-10-19'), Strike = 2080,Premium = 7.37, Right = 'PUT')
  SPX.Hedges.78= list(Expiry = as.Date('2017-10-19'), Strike = 2085,Premium = 7.57, Right = 'PUT')
  SPX.Hedges.79= list(Expiry = as.Date('2017-10-19'), Strike = 2090,Premium = 7.77, Right = 'PUT')
  SPX.Hedges.80= list(Expiry = as.Date('2017-10-19'), Strike = 2095,Premium = 7.97, Right = 'PUT')
  SPX.Hedges.81= list(Expiry = as.Date('2017-10-19'), Strike = 2100,Premium = 8.07, Right = 'PUT')
  SPX.Hedges.82= list(Expiry = as.Date('2017-10-19'), Strike = 2105,Premium = 8.27, Right = 'PUT')
  SPX.Hedges.83= list(Expiry = as.Date('2017-10-19'), Strike = 2110,Premium = 8.47, Right = 'PUT')
  SPX.Hedges.84= list(Expiry = as.Date('2017-10-19'), Strike = 2115,Premium = 8.67, Right = 'PUT')
  SPX.Hedges.85= list(Expiry = as.Date('2017-10-19'), Strike = 2120,Premium = 8.9, Right = 'PUT')
  SPX.Hedges.86= list(Expiry = as.Date('2017-10-19'), Strike = 2125,Premium = 9.1, Right = 'PUT')
  SPX.Hedges.87= list(Expiry = as.Date('2017-10-19'), Strike = 2130,Premium = 9.37, Right = 'PUT')
  SPX.Hedges.88= list(Expiry = as.Date('2017-10-19'), Strike = 2135,Premium = 9.57, Right = 'PUT')
  SPX.Hedges.89= list(Expiry = as.Date('2017-10-19'), Strike = 2140,Premium = 9.8, Right = 'PUT')
  SPX.Hedges.90= list(Expiry = as.Date('2017-10-19'), Strike = 2145,Premium = 10, Right = 'PUT')
  SPX.Hedges.91= list(Expiry = as.Date('2017-10-19'), Strike = 2150,Premium = 10.3, Right = 'PUT')
  SPX.Hedges.92= list(Expiry = as.Date('2017-10-19'), Strike = 2155,Premium = 10.5, Right = 'PUT')
  SPX.Hedges.93= list(Expiry = as.Date('2017-10-19'), Strike = 2160,Premium = 10.8, Right = 'PUT')
  SPX.Hedges.94= list(Expiry = as.Date('2017-10-19'), Strike = 2165,Premium = 11.03, Right = 'PUT')
  SPX.Hedges.95= list(Expiry = as.Date('2017-10-19'), Strike = 2170,Premium = 11.33, Right = 'PUT')
  SPX.Hedges.96= list(Expiry = as.Date('2017-10-19'), Strike = 2175,Premium = 11.63, Right = 'PUT')
  SPX.Hedges.97= list(Expiry = as.Date('2017-10-19'), Strike = 2180,Premium = 11.93, Right = 'PUT')
  SPX.Hedges.98= list(Expiry = as.Date('2017-10-19'), Strike = 2185,Premium = 12.23, Right = 'PUT')
  SPX.Hedges.99= list(Expiry = as.Date('2017-10-19'), Strike = 2190,Premium = 12.53, Right = 'PUT')
  SPX.Hedges.100= list(Expiry = as.Date('2017-10-19'), Strike = 2195,Premium = 12.86, Right = 'PUT')
  SPX.Hedges.101= list(Expiry = as.Date('2017-10-19'), Strike = 2200,Premium = 13.22, Right = 'PUT')
  SPX.Hedges.102= list(Expiry = as.Date('2017-10-19'), Strike = 2205,Premium = 13.56, Right = 'PUT')
  SPX.Hedges.103= list(Expiry = as.Date('2017-10-19'), Strike = 2210,Premium = 13.86, Right = 'PUT')
  SPX.Hedges.104= list(Expiry = as.Date('2017-10-19'), Strike = 2215,Premium = 14.16, Right = 'PUT')
  SPX.Hedges.105= list(Expiry = as.Date('2017-10-19'), Strike = 2220,Premium = 14.56, Right = 'PUT')
  SPX.Hedges.106= list(Expiry = as.Date('2017-10-19'), Strike = 2225,Premium = 14.96, Right = 'PUT')
  SPX.Hedges.107= list(Expiry = as.Date('2017-10-19'), Strike = 2230,Premium = 15.45, Right = 'PUT')
  SPX.Hedges.108= list(Expiry = as.Date('2017-10-19'), Strike = 2235,Premium = 15.82, Right = 'PUT')
  SPX.Hedges.109= list(Expiry = as.Date('2017-10-19'), Strike = 2240,Premium = 16.19, Right = 'PUT')
  SPX.Hedges.110= list(Expiry = as.Date('2017-10-19'), Strike = 2245,Premium = 16.62, Right = 'PUT')
  SPX.Hedges.111= list(Expiry = as.Date('2017-10-19'), Strike = 2250,Premium = 17.06, Right = 'PUT')
  SPX.Hedges.112= list(Expiry = as.Date('2017-10-19'), Strike = 2255,Premium = 17.48, Right = 'PUT')
  SPX.Hedges.113= list(Expiry = as.Date('2017-10-19'), Strike = 2260,Premium = 17.92, Right = 'PUT')
  SPX.Hedges.114= list(Expiry = as.Date('2017-10-19'), Strike = 2265,Premium = 18.42, Right = 'PUT')
  SPX.Hedges.115= list(Expiry = as.Date('2017-10-19'), Strike = 2270,Premium = 18.98, Right = 'PUT')
  SPX.Hedges.116= list(Expiry = as.Date('2017-10-19'), Strike = 2275,Premium = 19.42, Right = 'PUT')
  SPX.Hedges.117= list(Expiry = as.Date('2017-10-19'), Strike = 2280,Premium = 19.99, Right = 'PUT')
  SPX.Hedges.118= list(Expiry = as.Date('2017-10-19'), Strike = 2285,Premium = 20.58, Right = 'PUT')
  SPX.Hedges.119= list(Expiry = as.Date('2017-10-19'), Strike = 2290,Premium = 21.08, Right = 'PUT')
  SPX.Hedges.120= list(Expiry = as.Date('2017-10-19'), Strike = 2295,Premium = 21.68, Right = 'PUT')
  SPX.Hedges.121= list(Expiry = as.Date('2017-10-19'), Strike = 2300,Premium = 22.23, Right = 'PUT')
  SPX.Hedges.122= list(Expiry = as.Date('2017-10-19'), Strike = 2305,Premium = 22.95, Right = 'PUT')
  SPX.Hedges.123= list(Expiry = as.Date('2017-10-19'), Strike = 2310,Premium = 23.65, Right = 'PUT')
  SPX.Hedges.124= list(Expiry = as.Date('2017-10-19'), Strike = 2315,Premium = 24.28, Right = 'PUT')
  SPX.Hedges.125= list(Expiry = as.Date('2017-10-19'), Strike = 2320,Premium = 24.98, Right = 'PUT')
  SPX.Hedges.126= list(Expiry = as.Date('2017-10-19'), Strike = 2325,Premium = 25.75, Right = 'PUT')
  SPX.Hedges.127= list(Expiry = as.Date('2017-10-19'), Strike = 2330,Premium = 26.48, Right = 'PUT')
  SPX.Hedges.128= list(Expiry = as.Date('2017-10-19'), Strike = 2335,Premium = 27.28, Right = 'PUT')
  SPX.Hedges.129= list(Expiry = as.Date('2017-10-19'), Strike = 2340,Premium = 28.08, Right = 'PUT')
  SPX.Hedges.130= list(Expiry = as.Date('2017-10-19'), Strike = 2345,Premium = 28.87, Right = 'PUT')
  SPX.Hedges.131= list(Expiry = as.Date('2017-10-19'), Strike = 2350,Premium = 29.71, Right = 'PUT')
  SPX.Hedges.132= list(Expiry = as.Date('2017-10-19'), Strike = 2355,Premium = 30.61, Right = 'PUT')
  SPX.Hedges.133= list(Expiry = as.Date('2017-10-19'), Strike = 2360,Premium = 31.51, Right = 'PUT')
  SPX.Hedges.134= list(Expiry = as.Date('2017-10-19'), Strike = 2365,Premium = 32.44, Right = 'PUT')
  SPX.Hedges.135= list(Expiry = as.Date('2017-10-19'), Strike = 2370,Premium = 33.44, Right = 'PUT')
  SPX.Hedges.136= list(Expiry = as.Date('2017-10-19'), Strike = 2375,Premium = 34.41, Right = 'PUT')
  SPX.Hedges.137= list(Expiry = as.Date('2017-10-19'), Strike = 2380,Premium = 35.44, Right = 'PUT')
  SPX.Hedges.138= list(Expiry = as.Date('2017-10-19'), Strike = 2385,Premium = 36.54, Right = 'PUT')
  SPX.Hedges.139= list(Expiry = as.Date('2017-10-19'), Strike = 2390,Premium = 37.67, Right = 'PUT')
  SPX.Hedges.140= list(Expiry = as.Date('2017-10-19'), Strike = 2395,Premium = 38.94, Right = 'PUT')
  SPX.Hedges.141= list(Expiry = as.Date('2017-10-19'), Strike = 2400,Premium = 40.42, Right = 'PUT')
  SPX.Hedges.142= list(Expiry = as.Date('2017-10-19'), Strike = 2405,Premium = 41.44, Right = 'PUT')
  SPX.Hedges.143= list(Expiry = as.Date('2017-10-19'), Strike = 2410,Premium = 42.74, Right = 'PUT')
  SPX.Hedges.144= list(Expiry = as.Date('2017-10-19'), Strike = 2415,Premium = 44.1, Right = 'PUT')
  SPX.Hedges.145= list(Expiry = as.Date('2017-10-19'), Strike = 2420,Premium = 45.5, Right = 'PUT')
  SPX.Hedges.146= list(Expiry = as.Date('2017-10-19'), Strike = 2425,Premium = 46.97, Right = 'PUT')
  SPX.Hedges.147= list(Expiry = as.Date('2017-10-19'), Strike = 2430,Premium = 48.43, Right = 'PUT')
  SPX.Hedges.148= list(Expiry = as.Date('2017-10-19'), Strike = 2435,Premium = 50.1, Right = 'PUT')
  
  
  SPX.Assets =lapply(1:148,function(x) return (get(paste0('SPX.Hedges.', x))))
}



###################################################################
#    Set Parameters
###################################################################
{
  BackwardWaveLen  = 0
  ForwardWaveLen  = 1
  WaveLen = BackwardWaveLen + ForwardWaveLen + 1
  Include.SPX = 1
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
    SPX.DF = data.frame(Date = as.Date(SPX.Raw$Date,'%Y-%m-%d'),Price = as.numeric(SPX.Raw$Close))
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
  plot(x = Final.DF.oF$Date, y = SPX.DF$Price[match(Final.DF.oF$Date,SPX.DF$Date)], type = 'l',col = 'blue',lwd = 1.5,axes = F)
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
  # rm(mGARCHsim)
  # rm(Simmed.Path)
  # rm(List.ShockMatrices)
  # gc()
  
  ############### SELECTED PRICE PLOTS #############
  Selected.Plot = function(symbol,index)
  {
    ####### Read in data  #############
    setwd(paste0(Parent.WD,'/',symbol ))
    Data_Prices = read.csv('Prices.csv', T)
    LastClose = Data_Prices$Close[as.Date(Data_Prices$Date,'%Y-%m-%d') == max(Final.DF$Date)]
    
    plot(LastClose*cumprod(1+Simmed.Path.List[[symbol]][,index]),type = 'l')
    
    return(0)
  }
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

  Payoffs = function(asset){
    sim.Prices = Calculate_Fitted.Expiry.Prices(asset[['symbol']],asset[['Expiry']])
    return(sapply(sim.Prices,payoff,K = asset[['Strike']],Premium = asset[['Premium']], Right = asset[['Right']]))
  }
  
  Payoffs.SPX = function(asset.SPX){
    sim.Prices = Calculate_Fitted.Expiry.Prices('SPX',asset.SPX[['Expiry']])
    return(-sapply(sim.Prices,payoff,K = asset.SPX[['Strike']],Premium = asset.SPX[['Premium']], Right = asset.SPX[['Right']]))
  }
  
}


###################################################################
#       Hedging Calculation
###################################################################
{
  Loss.Threshold = -0.7
  
  ######## Prepare the portfolio ########
  Portfolio.Payoff.Vector = matrix(sapply(assets,Payoffs),byrow = F,ncol = length(assets)) %*%
    sapply(assets,function(asset) return(-asset[['Position']]))
  
  Portfolio.Margin.scalar = 416707 #input this number
  
  ######## Prepare the Hedges ########
  Hedge.Payoff.Matrix = matrix(sapply(SPX.Assets,Payoffs.SPX),byrow = F,ncol = length(SPX.Assets))
  
  Hedge.Premium.Vector = sapply(SPX.Assets,function(x) return(x[['Premium']]))
  
  ######## Calculate the Hedgs ########
  Hedged.Return.Vector = function(Hedge.N){
    Total.Payoff.Vector = Portfolio.Payoff.Vector + Hedge.Payoff.Matrix %*% Hedge.N
    Total.Premium.Margin = Portfolio.Margin.scalar / 780 + Hedge.Premium.Vector %*% Hedge.N
    return(Total.Payoff.Vector/as.numeric(Total.Premium.Margin))
  }
  
  ######## Define ratio ########
  WL.Ratio = function(Hedge.N){
    Returns =  Hedged.Return.Vector(Hedge.N)
    Ex.Win = mean(Returns[Returns>0])
    Ex.Shortfall = -ifelse(is.na(mean(Returns[Returns<Loss.Threshold])),
                           Loss.Threshold,
                           mean(Returns[Returns<Loss.Threshold]))
    # return(Ex.Win/Ex.Shortfall)
    return(sum(Returns>0))
  }
  
  ######## Analyse results ########
  Hedge.Sample.Matrix = diag(148)
  WL.Ratio.Vector = apply(Hedge.Sample.Matrix,1,WL.Ratio)
  Hedge.Sorted = Hedge.Sample.Matrix[order(WL.Ratio.Vector,decreasing = T),]
  Mean.Return = apply(Hedge.Sorted,1,function(x){
    return(mean(Hedged.Return.Vector(x)))
  })
  Median.Return = apply(Hedge.Sorted,1,function(x){
    return(mean(Hedged.Return.Vector(x)))
  })

  #function(x){-WL.Ratio(x)}
  ######## Genetic Algorithm ########
  Model.MaxWL = DEoptim(fn=function(x){-WL.Ratio(x)},lower=rep(-1,length(SPX.Assets)),upper = rep(1,length(SPX.Assets)),
                            control = DEoptim.control(NP = 2000,itermax = 1000, trace = T,p = .1),fnMap = round)

}
