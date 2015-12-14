public class NistTimeTest
{
    public static bool isParseCorrect()
    {
        NistTime time = new NistTime();
        DateTime result = time.ParseNistAnswer("57368 15-12-12 09:11:54 00 0 0 778.9 UTC(NIST) *");
        DateTime control = new DateTime(2015, 12, 12, 9, 11, 54, DateTimeKind.Utc);
        control = control.AddMilliseconds(778.9);
        return control.Equals(result);
    }
}