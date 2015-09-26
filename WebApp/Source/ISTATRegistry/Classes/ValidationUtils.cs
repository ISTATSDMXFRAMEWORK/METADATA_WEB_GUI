using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ISTATRegistry
{

    /*
     * Nome: ValidationUtils
     * 
     * Descrizione:
     * Classe di gestione della validazione sui controlli 
     */ 
    public class ValidationUtils
    {
        /*
         * Nome: CheckIdFormat
         * 
         * Descrizione:
         * Controlla che l'id passato non contenga roba strana, tramite reqexp
         */
        static public bool CheckIdFormat( string idString )
        {
            // Verifico che la stringa non sia vuota
            if ( !idString.Trim().Equals( string.Empty ) && !idString.Trim().Contains( ' ' ) ) 
            {
                const string idFormatPattern = @"^[a-zA-Z0-9_]*$";
                Regex rgx = new Regex( idFormatPattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches( idString );
                if (matches.Count > 0)  //  Se la reqexp è soddisfatta ...
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /*
         * Nome: CheckVersion 
         * 
         * Descrizione:
         * Controlla che la versione passata sia nel formato A.B o A.B.C dove
         * A deve essere un numero tra 1 e 9 mentre sia B che C devono essere contenuti
         * tra 0 e 9
         */
        static public bool CheckVersionFormat( string versionString )
        {
            // Verifico che la versione non sia vuota
            if ( !versionString.Trim().Equals( string.Empty ) )
            {
                string[] tmpArray = versionString.Split( '.' );
                // Il numero di cifre della versione devono essere 2 o 3
                if ( tmpArray.Length == 2 || tmpArray.Length == 3 )
                {
                    if ( tmpArray.Length == 2 )
                    {
                        int major = 0, minor = 0;
                        if ( int.TryParse(  tmpArray[0], out major ) && int.TryParse(  tmpArray[1], out minor ) )
                        {
                            // Se sono 2 la prima deve essere contenuta tra 0 e 9
                            if ( !(Convert.ToInt32( tmpArray[0] ) < 10 &&  Convert.ToInt32( tmpArray[0] ) > 0 ) )
                            {
                                return false;
                            }
                            // La seconda tra 1 e 9
                            if ( !(Convert.ToInt32( tmpArray[1] ) < 10 &&  Convert.ToInt32( tmpArray[1] ) >= 0 ) )
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }

                        return true;
                    }
                    else
                    {
                        int major = 0, middle = 0, minor = 0 ;
                        if ( int.TryParse(  tmpArray[0], out major ) && int.TryParse(  tmpArray[1], out middle ) && int.TryParse(  tmpArray[2], out minor ) )
                        {
                            // Se sono 3 la prima deve essere tra 0 e 9 ...
                            if ( !(Convert.ToInt32( tmpArray[0] ) < 10 &&  Convert.ToInt32( tmpArray[0] ) > 0 ) )
                            {
                                return false;
                            }

                            // ... mentre la seconda e la terza tra 1 e 9
                            if ( !(Convert.ToInt32( tmpArray[1] ) < 10 &&  Convert.ToInt32( tmpArray[1] ) >= 0 ) )
                            {
                                return false;
                            }

                            if ( !(Convert.ToInt32( tmpArray[2] ) < 10 &&  Convert.ToInt32( tmpArray[2] ) >= 0 ) )
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /*
         * Nome: CheckUriFormat
         * 
         * Descrizione:
         * Controlla che la Uri passata sia nel formato URL corretto, tramite reqexp
         */
        static public bool CheckUriFormat( string uriString )
        {
            // Se l'URI è vuoto allora viene considerato accettabile
            if ( uriString.Trim().Equals( string.Empty ) )
            {
                return true;
            }
            else
            {
                // altrimenti vengono effettuati i check
                //const string urlFormatPattern = @"((mailto\:|(news|(ht|f)tp(s?))\://){1}\S+)";
                const string urlFormatPattern = @"((((ht|f)tp(s?))\://){1}\S+)";
                Regex rgx = new Regex( urlFormatPattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches( uriString );
                if (matches.Count > 0 || System.Uri.IsWellFormedUriString( uriString, UriKind.Absolute ) )  //  Se la reqexp è soddisfatta ...
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /*
         * Nome: CheckUriFormatNoRegExp
         * 
         * Descrizione:
         * Controlla che la Uri passata sia nel formato URL corretto, tramite creazione
         * e restituzione di un oggetto. Restituisce null in caso di errore nella creazione 
         * dell'istanza
         */
        static public bool CheckUriFormatNoRegExp( string uriString )
        {
            return System.Uri.IsWellFormedUriString( uriString, UriKind.Absolute );
        }

         /*
         * Nome: CheckDates
         * 
         * Descrizione:
         * Controlla il corretto inserimento delle date di validità
         */
        static public bool CheckDates( string dateFrom, string dateTo )
        {
            try
            {
                DateTime convertedDateFrom, convertedDateTo;
                // Converto le stringhe in oggetti Datetime
                convertedDateFrom = DateTime.ParseExact( dateFrom, "dd/mm/yyyy", CultureInfo.InvariantCulture );
                convertedDateTo = DateTime.ParseExact( dateTo, "dd/mm/yyyy", CultureInfo.InvariantCulture );
                // Verifico che la data iniziale sia minore o uguale della data finale
                if ( convertedDateFrom <= convertedDateTo )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch( FormatException ex )
            {
                // il parsing non è avvenuto correttamente
                return false;
            }
        }

        /*
         * Nome: CheckDateFormat
         * 
         * Descrizione:
         * Controlla il corretto formato della data passata in input
         */
        static public bool CheckDateFormat( string date )
        {
            try
            {
                DateTime result;
                result = DateTime.ParseExact( date, "dd/mm/yyyy", CultureInfo.InvariantCulture );
                return true;
            }
            catch( FormatException ex )
            {
                // il parsing non è avvenuto correttamente
                return false;
            }
        }

        /// <summary>
        /// Controlla se il valore passato in input è di tipo Integer
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        static public bool CheckIntegerFormat(string intValue)
        {
            int i;
            return Int32.TryParse(intValue, out i);
        }

    }
}