import numpy as np
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.engine_configuration_channel import EngineConfigurationChannel


env = UnityEnvironment(file_name=None, seed = 1, side_channels = [],base_port=5004)
env.reset()

list_agents = env.get_agent_groups()
agent = list_agents[0]

#initialization of the input parameters to check
inpchecker = np.zeros((1,2))
inpchecker[0][0] = 1 #throttle  1 = forward; 2 = reverse; 0 = none
inpchecker[0][1] = 1 #steer     1 = right; 2 = left; 0 = none
inpchecker = np.asarray(inpchecker)


curr_state  = env.reset()

# channel = EngineConfigurationChannel()
# channel.set_configuration_parameters(time_scale=20.0)

while True:
    env.set_actions(list_agents[0], inpchecker)
    env.step()
    step_result = env.get_step_result(list_agents[0])
    obs = step_result.obs
    rewards = step_result.reward
        


env.close()