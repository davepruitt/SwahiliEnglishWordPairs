using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanAcceleratedLearning.Helpers;

namespace HumanAcceleratedLearning.Models
{
    public class Stage
    {
        #region Constructor

        public Stage()
        {
            //empty
        }

        #endregion

        #region Properties

        public string StageName { get; set; } = string.Empty;

        public string StageDescription { get; set; } = string.Empty;

        public List<StageSegment> StageSegments { get; set; } = new List<StageSegment>();
        
        #endregion

        #region Static methods

        private static (string, List<Tuple<string, string>>) ParseParametersFromString(string line)
        {
            string result1 = string.Empty;
            List<Tuple<string, string>> result_parameters = new List<Tuple<string, string>>();

            //Split the line where commas are located
            var line_separated = line.Split(new char[] { ',' }).ToList();

            //The first piece will be the "command"
            result1 = line_separated[0].Trim();

            //Handle the special case of a "loop" command
            if (result1.Equals("loop"))
            {
                if (line_separated.Count > 1)
                {
                    line_separated[1] = line_separated[1].Trim();

                    if (line_separated[1].Equals("end"))
                    {
                        result_parameters.Add(new Tuple<string, string>("end", "end"));
                    }
                    else
                    {
                        result_parameters.Add(new Tuple<string, string>("count", line_separated[1]));
                    }
                }
            }
            else
            {
                //All other pieces will be parameters for the command
                for (int i = 1; i < line_separated.Count; i++)
                {
                    string this_parameter = line_separated[i].Trim();

                    //Split the parameter based on where the equal sign is located
                    var parameter_split = this_parameter.Split(new char[] { '=' }).ToList();
                    if (parameter_split.Count == 2)
                    {
                        result_parameters.Add(new Tuple<string, string>(parameter_split[0].Trim(), parameter_split[1].Trim()));
                    }
                }
            }

            return (result1, result_parameters);
        }

        private static int StructurePhases (List<string> lines, int line_index, PhaseNode current_parent)
        {
            while (line_index < lines.Count)
            {
                (string command, var parameters) = ParseParametersFromString(lines[line_index]);

                PhaseNode result = new PhaseNode()
                {
                    Command = command,
                    Parameters = parameters
                };

                if (result.Command.Equals("loop"))
                {
                    if (result.Parameters.Count > 0)
                    {
                        //If this is a new loop node
                        if (result.Parameters[0].Item1.Equals("count"))
                        {
                            //Add this new loop to the parent's list of phases
                            current_parent.Children.Add(result);

                            //Recursively parse the children of this new loop
                            line_index = StructurePhases(lines, line_index + 1, result);
                        }
                        else if (result.Parameters[0].Item1.Equals("end"))
                        {
                            //If the node is a "loop end" node then just return. Don't even add this node to the parent's children
                            return (line_index + 1);
                        }
                    }
                }
                else if (result.Command.Equals("segment"))
                {
                    if (result.Parameters.Count > 0)
                    {
                        //Get the control parameter
                        var ctl_param = result.Parameters.Where(x => x.Item1.Equals("control")).FirstOrDefault();
                        if (ctl_param != null)
                        {
                            if (ctl_param.Item2.Equals("start"))
                            {
                                //Add this new loop to the parent's list of phases
                                current_parent.Children.Add(result);

                                //Recursively parse the children of this new loop
                                line_index = StructurePhases(lines, line_index + 1, result);
                            }
                            else if (ctl_param.Item2.Equals("end"))
                            {
                                //If the node is a "loop end" node then just return. Don't even add this node to the parent's children
                                return (line_index + 1);
                            }
                        }
                    }
                }
                else
                {
                    //Add the new node to the current parent
                    current_parent.Children.Add(result);

                    //Go on to the next line
                    line_index++;
                }
            }

            return 0;
        }

        private static void ExpandPhaseNodes (List<StageSegment> segments, StageSegment current_segment, PhaseNode parent)
        {
            //Check to see if the parent node is a loop. If so, we potentially need to iterate over all the children multiple times.
            int loop_count = 1;
            if (parent.Command.Equals("loop"))
            {
                bool success = Int32.TryParse(parent.Parameters[0].Item2, out int lc);
                if (success)
                {
                    loop_count = lc;
                }
            }

            for (int c = 0; c < loop_count; c++)
            {
                //Iterate over all children
                for (int i = 0; i < parent.Children.Count; i++)
                {
                    var this_child = parent.Children[i];

                    if (this_child.Command.Equals("loop"))
                    {
                        //If this is a loop node, expand its children
                        ExpandPhaseNodes(segments, current_segment, this_child);
                    }
                    else if (this_child.Command.Equals("segment"))
                    {
                        //Initialize a new segment
                        current_segment = new StageSegment();
                        current_segment.InitializeSegmentFromParameters(this_child.Parameters);

                        //Add this new segment to the list of all segments
                        segments.Add(current_segment);

                        //Expand the children of this segment
                        ExpandPhaseNodes(segments, current_segment, this_child);
                    }
                    else
                    {
                        //Distinguish what kind of phase we are parsing
                        Phase new_phase = null;
                        if (this_child.Command.Equals("instructions"))
                        {
                            new_phase = new Phase_Instructions();
                        }
                        else if (this_child.Command.Equals("block"))
                        {
                            new_phase = new Phase_Block();
                        }
                        else if (this_child.Command.Equals("distraction"))
                        {
                            new_phase = new Phase_Distraction();
                        }
                        else if (this_child.Command.Equals("study"))
                        {
                            new_phase = new Phase_Study();
                        }
                        else if (this_child.Command.Equals("test"))
                        {
                            new_phase = new Phase_Test();
                        }
                        else if (this_child.Command.Equals("objectlocation_study"))
                        {
                            new_phase = new Phase_ObjectLocationStudy();
                        }
                        else if (this_child.Command.Equals("objectlocation_test"))
                        {
                            new_phase = new Phase_ObjectLocationTest();
                        }

                        if (new_phase != null)
                        {
                            //Initialize the phase based on the input parameters
                            new_phase.InitializePhaseFromParameters(this_child.Parameters, c + 1);

                            //Add the new phase to the current segment
                            if (current_segment != null)
                            {
                                current_segment.Phases.Add(new_phase);
                            }
                        }
                    }
                }
            }
        }

        public static Stage LoadStageFromFile (string file_name)
        {
            //Create an empty stage that will be our result
            Stage result = new Stage();
            
            //Grab all the lines from the file
            var file_lines = ConfigurationFileLoader.LoadConfigurationFile(file_name);

            for (int i = 0; i < file_lines.Count; i++)
            {
                var this_line = file_lines[i].Trim();
                if (this_line.Equals("BEGIN STAGE"))
                {
                    var file_lines_to_use = file_lines.GetRange(i + 1, file_lines.Count - i - 1).ToList();

                    //Parse the rest of the lines of the file
                    if (file_lines_to_use.Count > 0)
                    {
                        //Create a root phase node
                        PhaseNode root = new PhaseNode() { Command = "root" };

                        //Structure the list of commands
                        StructurePhases(file_lines_to_use, 0, root);

                        //Expand the phase node structure by instantiating each phase and putting them into a flat list
                        ExpandPhaseNodes(result.StageSegments, null, root);
                    }

                    //Break out of the loop. We are done.
                    break;
                }
                else
                {
                    var line_parts = this_line.Split(new char[] { ':' }, 2).ToList();
                    if (line_parts.Count >= 2)
                    {
                        var param_name = line_parts[0].Trim();
                        var param_val = line_parts[1].Trim();

                        if (param_name.Equals("Stage Name"))
                        {
                            result.StageName = param_val;
                        }
                        else if (param_name.Equals("Stage Description"))
                        {
                            result.StageDescription = param_val;
                        }
                    }
                }
            }
            
            //Return the new stage
            return result;
        }

        #endregion
    }
}
