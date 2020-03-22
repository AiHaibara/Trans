from tensor2tensor.utils import registry
from tensor2tensor import problems
from tensor2tensor.bin import t2t_trainer
import sys

# print(problems.available()) #Show all problems


t2t_trainer.main(sys.argv)
