# SwahiliEnglishWordPairs

This program was written to assess the rate of learning of Swahili/English word pairs. This can be used for any sort of clinical study, but we specifically used it to investigate whether vagus nerve stimulation (VNS) could enhance learning of the word pairs.

The program is written in C#. It consists of a "study phase" and a "test phase". During the "study phase", Swahili/English word pairs are displayed for 5 seconds each. During the "test phase" a Swahili word is displayed, and the user has 8 seconds to respond.

This program doesn't require any additional hardware to run. It does require audio input to determine certain times to advance if the VNS setting is turned on. It also is best to run on a dual-monitor setup (one for the experimenter and one for the test subject).

