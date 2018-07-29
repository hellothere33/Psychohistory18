#from fuzzywuzzy import fuzz

class uDataGen(object):
    """Generates for test purposes artificial strings, vectors, arrays"""
    
    def getTestText():
        texte = "Michelee C Bartolo-CMG Financial Representative - Michelee Bartolo CFNP Michelee C Bartolo - CMG Financial Representative, Rhonda S Alcazar"
        return texte

    def getTestTextArray():
        str = []
        str.append(["Michelee C Bartolo-CMG Financial Representative - Michelee Bartolo",
                    "CFNP	Michelee C Bartolo - CMG Financial Representative"])
        str.append(["Rhonda S Alcazar - Diversified Mortgage Group Financial Representative",
                    "Wendy Tannenbaum - Diversified Mortgage Group Financial Representative"])
        str.append(["7007 Wyoming Blvd NE , Suite F-5, Albuquerque, New Mexico, 87019",
                    "7007 Wyoming Blvd NE"])
        str.append(["albuquerque",
                    "Albuquerque"])
        str.append(["nm",
                    "NM"])
        str.append(["87019",
                    "87019"])
        return str

    # def TestStringDistance_FuzzyWuzzy():
        # str = uDataGen.getTestTextArray()
        # print("fuzz.ratio ------------------------------------")
        # for s in str:
            # res = fuzz.ratio(s[0], s[1])
            # print("%i: %s ~ %s" % (res, s[0], s[1]))
        # print("fuzz.partial_ratio ------------------------------------")
        # for s in str:
            # res = fuzz.partial_ratio(s[0], s[1])
            # print("%i: %s ~ %s" % (res, s[0], s[1]))
        # print("fuzz.token_sort_ratio ------------------------------------")
        # for s in str:
            # res = fuzz.token_sort_ratio(s[0], s[1])
            # print("%i: %s ~ %s" % (res, s[0], s[1]))
        # print("fuzz.token_set_ratio ------------------------------------")
        # for s in str:
            # res = fuzz.token_set_ratio(s[0], s[1])
            # print("%i: %s ~ %s" % (res, s[0], s[1]))
        # #===================================================================
        # # Fuzzy Wuzzy's functions
        # #fuzz.ratio("this is a test", "this is a test!")
        # #    97
        # #fuzz.partial_ratio("this is a test", "this is a test!")
        # #    100
        # #fuzz.ratio("fuzzy wuzzy was a bear", "wuzzy fuzzy was a bear")
        # #    91
        # #fuzz.token_sort_ratio("fuzzy wuzzy was a bear", "wuzzy fuzzy was a bear")
        # #    100
        # #fuzz.token_sort_ratio("fuzzy was a bear", "fuzzy fuzzy was a bear")
        # #    84
        # #fuzz.token_set_ratio("fuzzy was a bear", "fuzzy fuzzy was a bear")
        # #    100
        # #process.extractOne("System of a down - Hypnotize - Heroin", songs)
        # #    ('/music/library/good/System of a Down/2005 - Hypnotize/01 - Attack.mp3', 86)
        # #process.extractOne("System of a down - Hypnotize - Heroin", songs, scorer=fuzz.token_sort_ratio)
        # #    ("/music/library/good/System of a Down/2005 - Hypnotize/10 - She's Like Heroin.mp3", 61)

