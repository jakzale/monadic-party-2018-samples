module Main where

  import Prelude hiding (getContents)
  
  import Control.Monad (liftM)
  import Data.ByteString.Lazy.Internal (ByteString(..), defaultChunkSize)
  
  import Control.Exception (bracketOnError, onException)
  import System.Environment (getEnv)
  import Network.Socket hiding (send, sendTo, recv, recvFrom)
  
  import System.IO.Unsafe (unsafeInterleaveIO)
  
  import qualified Data.ByteString as B
  
  import qualified Network.Socket.ByteString.Lazy as L
  import qualified Data.ByteString.Lazy.Char8 as C
  
  import qualified Network.Socket.ByteString as N
  
  import Debug.Trace (traceIO)
  
  -- Custom contents for reading from socket that will be closed afterwards
  getContents :: Socket -> IO ByteString
  getContents sock = loop where
    loop = unsafeInterleaveIO $ do
      s <- N.recv sock defaultChunkSize
      if B.null s
        then close sock >> return Empty
        else Chunk s `liftM` loop
  
  -- Handle input from Azure
  azureInput :: String -> IO ByteString
  azureInput inputName =
    do
      sockPath <- getEnv inputName
      traceIO $ "Input Socket Path: " ++ sockPath
      sock <- socket AF_UNIX Stream defaultProtocol
      connect sock (SockAddrUnix sockPath)
      getContents sock
  
  azIn :: IO ByteString
  azIn = azureInput "AZ_INPUT"
  
  -- Handle output to Azure
  azureOutput :: String -> ByteString -> IO ()
  azureOutput outputName contents =
    do
      sockPath <- getEnv outputName
      traceIO $ "Output Socket Path: " ++ sockPath
      sock <- socket AF_UNIX Stream defaultProtocol
      connect sock (SockAddrUnix sockPath)
      L.sendAll sock contents
  
  
  azOut :: ByteString -> IO ()
  azOut = azureOutput "AZ_OUTPUT"
  
  
  -- Example of pure function to be run
  hello :: ByteString -> ByteString
  hello req =
    C.pack message
    where
      name = C.unpack req
      message = "Hello, " ++ name
      
      
  main :: IO ()
  main =
    do
      putStrLn "Saying hello from Haskell"
      name <- azIn
      azOut $ hello name
  
  