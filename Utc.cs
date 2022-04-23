using System;
using System.Globalization;

namespace Tore.Core {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Utc [Static]                                            <summary>
        USAGE:                                                          <br/>
            Contains static utility methods for DateTime conversions.   <para/>
            String, Seconds, Milliseconds return <b>UtcNow</b> values.  <para/>
            CultureInfo is <b>CultureInfo.InvariantCulture.</b>         </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Utc {

        private static CultureInfo timeProvider = CultureInfo.InvariantCulture;

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: String [static]                                             <summary>
          TASK:                                                             <br/>
                Returns UTC current time as string.                         <para/>
          RETV:                                                             <br/>
                    : string : UTC time string : "yyyyMMddHHmmssfff".       </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string String() {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", timeProvider);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Seconds [static]                                            <summary>
          TASK:                                                             <br/>
                Returns number of seconds since unix epoch + offset.        <para/>
          ARGS:                                                             <br/>
                offset  : long  : Offset seconds to Add : DEF : 0.          <para/>
          RETV:                                                             <br/>
                        : long  : Seconds since unix epoch + offset.        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static long Seconds(long offset = 0) {
            DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
            return dto.ToUnixTimeSeconds() + offset;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Milliseconds [static]                                       <summary>
          TASK:                                                             <br/>
                Returns number of milliseconds since unix epoch + offset.   <para/>
          ARGS:                                                             <br/>
                offset  : long  : Offset milliseconds to Add : DEF : 0.     <para/>
          RETV:                                                             <br/>
                        : long  : milliseconds since unix epoch + offset.   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static long Milliseconds(long offset = 0) {
            DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
            return dto.ToUnixTimeMilliseconds() + offset;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StringToDateTime [static]                                   <summary>
          TASK:                                                             <br/>
                Returns string formatted time as DateTime.                  <para/>
          ARGS:                                                             <br/>
                time    : string    : Time string: "yyyyMMddHHmmssfff".     <para/>
          RETV:                                                             <br/>
                        : DateTime  : DateTime translation of time string.  </summary> 
        ————————————————————————————————————————————————————————————————————————————*/
        public static DateTime StringToDateTime(string time) {
            return DateTime.ParseExact(time, "yyyyMMddHHmmssfff", timeProvider);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SecondsToDateTime [static]                                  <summary>
          TASK:                                                             <br/>
                Returns seconds as DateTime.                                <para/>
          ARGS:                                                             <br/>
                seconds : long      : Time as seconds since unix epoch.     <para/>
          RETV:                                                             <br/>
                        : DateTime  : DateTime translation of seconds.      </summary> 
        ————————————————————————————————————————————————————————————————————————————*/
        public static DateTime SecondsToDateTime(long seconds) {
            return DateTimeOffset.FromUnixTimeSeconds(seconds).DateTime;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: MillisecondsToDateTime [static]                             <summary>
          TASK:                                                             <br/>
                Returns milliseconds as DateTime.                           <para/>
          ARGS:                                                             <br/>
                milliseconds : long : Time as milliseconds since unix epoch.<para/>
          RETV:                                                             <br/>
                            : DateTime  : DateTime translation of seconds.  </summary> 
        ————————————————————————————————————————————————————————————————————————————*/
        public static DateTime MillisecondsToDateTime(long milliseconds) {
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime;
        }

    }
}
